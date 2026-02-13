using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;

namespace PingedGu.Controllers
{
    public class HomeController : Controller
    {
        //1. The code here is for loading data from the database to the web app

        private readonly ILogger<HomeController> _logger;
        private readonly WebAppDbContext _context;

        //Constructor
        public HomeController(ILogger<HomeController> logger, WebAppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //-------------------

        public async Task<IActionResult> Index()
        {
            //1. The code here is for loading data from the database to the web app
            var allPosts = await _context.Posts
                .Include(n => n.User)
                .ToListAsync();
            //
            return View(allPosts); 
        } 
    }
}
