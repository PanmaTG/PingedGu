using Microsoft.EntityFrameworkCore;

namespace PingedGu.Data
{
    public class WebAppDbContext:DbContext
    {
        // Constructor Method. WebAppDbContext used as a parameter inside <>.
        // :base keyword points to the :DbContext 
        public WebAppDbContext(DbContextOptions<WebAppDbContext> options):base(options)
        {
            
        }
    }
}
 