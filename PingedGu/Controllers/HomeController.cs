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
                .Include(n => n.Likes)
                .Include(n => n.Comments).ThenInclude(n => n.User)
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

            //For checking and saving of image
            if(post.Image != null && post.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if(post.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/uploaded");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using(var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await post.Image.CopyToAsync(stream);
                    }

                    //Set the ImageUrl property of the new post to the relative path of the saved image
                    newPost.ImageUrl = "/images/uploaded/" + fileName;
                }
            }

            //Adds the post to the database and saves the changes
            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //Post Like
        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeViewModel postLikeViewModel)
        {
            int loggedInUserId = 1;
            
            //Check if user has already liked the post
            var like = await _context.Likes
                .Where(l => l.PostId == postLikeViewModel.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postLikeViewModel.PostId,
                    UserId = loggedInUserId
                };

                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Post Comment
        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentViewModel postCommentViewModel)
        {
            int loggedInUserId = 1;

            var newComment = new Comment()
            {
                UserId = loggedInUserId,
                PostId = postCommentViewModel.PostId,
                Content = postCommentViewModel.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
