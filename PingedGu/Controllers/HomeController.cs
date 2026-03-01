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
            int loggedInUserId = 1;
            //1. The code here is for loading data from the database to the web app
            var allPosts = await _context.Posts
                .Where(n => !n.IsPrivate || n.UserId == loggedInUserId)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Favorites)
                .Include(n => n.Comments).ThenInclude(n => n.User)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
            //
            return View(allPosts); 
        }

        //Creating Post
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

        // Post - Report
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportViewModel postReportViewModel)
        {
            int loggedInUserId = 1;

            var newReport = new Report()
            {
                UserId = loggedInUserId,
                PostId = postReportViewModel.PostId,
                DateCreated = DateTime.UtcNow,
            };

            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Post - Set As Private
        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityViewModel postVisibilityViewModel)
        {
            int loggedInUserId = 1;

            // Get post id and loggedin user id
            var post = await _context.Posts
                .FirstOrDefaultAsync(l => l.Id == postVisibilityViewModel.PostId && l.UserId == loggedInUserId);

            if (post != null)
            {
                post.IsPrivate = !post.IsPrivate;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Post Favorite/Bookmark
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteViewModel postFavoriteViewModel)
        {
            int loggedInUserId = 1;

            //Check if user has already favorited/bookmarked the post
            var favorite = await _context.Favorites
                .Where(l => l.PostId == postFavoriteViewModel.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postFavoriteViewModel.PostId,
                    UserId = loggedInUserId
                };

                await _context.Favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Post Comment/Reply
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

        // Remove Comment/Reply
        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentViewModel removeCommentViewModel)
        {
            var commentDb = await _context.Comments.FirstOrDefaultAsync(c => c.Id == removeCommentViewModel.CommentId);

            if (commentDb != null)
            {
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
