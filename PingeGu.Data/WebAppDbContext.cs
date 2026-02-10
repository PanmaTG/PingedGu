using Microsoft.EntityFrameworkCore;
using PingedGu.Data.Models;


namespace PingedGu.Data
{
    public class WebAppDbContext:DbContext
    {
        // Constructor Method. WebAppDbContext used as a parameter inside <>.
        // :base keyword points to the :DbContext 
        public WebAppDbContext(DbContextOptions<WebAppDbContext> options):base(options)
        {
            
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);
        }
    }
}
 