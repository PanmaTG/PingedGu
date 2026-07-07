using Microsoft.EntityFrameworkCore;
using PingedGu.Data;
using PingedGu.Data.Helpers;
using PingedGu.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); 

//Database Config
string dbConnectionString = builder.Configuration.GetConnectionString("Default") ?? "";

//Services Config
builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<ITrendingsService, TrendingsService>();
builder.Services.AddScoped<IStoriesService, StoriesService>();
builder.Services.AddScoped<IFilesService, FilesService>();

//WebAppDbContext is the name of the class I created inside the Data folder
builder.Services.AddDbContext<WebAppDbContext>(options => options.UseSqlServer(dbConnectionString));


var app = builder.Build();

//Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WebAppDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbInitializer.SeedAsync(dbContext);
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
