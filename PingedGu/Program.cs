using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;
using PingedGu.Data.Helpers;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); 

//Database Config
string dbConnectionString = builder.Configuration.GetConnectionString("Default") ?? "";
//WebAppDbContext is the name of the class I created inside the Data folder
builder.Services.AddDbContext<WebAppDbContext>(options => options.UseSqlServer(dbConnectionString));

//Services Config
builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<ITrendingsService, TrendingsService>();
builder.Services.AddScoped<IStoriesService, StoriesService>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IFriendsService, FriendsService>();

//Identity Config - Auth
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    //Password Settings
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true; 
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
                .AddEntityFrameworkStores<WebAppDbContext>()
                .AddDefaultTokenProviders();

//Cookie Config - Auth
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Authentication/AccessDenied";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Auth:Google:ClientId"] ?? "";
                    options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"] ?? "";
                    options.CallbackPath = "/signin-google";
                }).AddGitHub(options =>
                {
                    options.ClientId = builder.Configuration["Auth:GitHub:ClientId"] ?? "";
                    options.ClientSecret = builder.Configuration["Auth:GitHub:ClientSecret"] ?? "";
                    options.CallbackPath = "/signin-github";
                    options.Scope.Add("user:email");
                });

builder.Services.AddAuthorization();

// Register SignalR services
builder.Services.AddSignalR();

var app = builder.Build();

//Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WebAppDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbInitializer.SeedAsync(dbContext);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
     await DbInitializer.SeedUsersAndRolesAsync(userManager, roleManager);
}
    
//---------------
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Hub for SignalR
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
