using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;
using PingedGu.Data.Models;
using PingedGu.ViewModels.Timeline;

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
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
            //
            return View(allPosts); 
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel post)
        {
            //Get the logged in user
            int loggedInUser = 1;

            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ImageUrl = "",
                NumOfReports = 0,
                UserId = loggedInUser
            };

            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
