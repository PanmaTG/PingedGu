using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;
using PingedGu.Data.Helpers;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Timeline;
using System.Diagnostics;

namespace PingedGu.Controllers
{
    public class HomeController : Controller
    {
        //1. The code here is for loading data from the database to the web app

        private readonly ILogger<HomeController> _logger;
        private readonly IPostsService _postsService;
        private readonly ITrendingsService _trendingsService;

        //Constructor
        public HomeController(ILogger<HomeController> logger, 
            IPostsService postsService, 
            ITrendingsService trendingsService)
        {
            _logger = logger;
            _postsService = postsService;
            _trendingsService = trendingsService;
        }

        //-------------------

        public async Task<IActionResult> Index()
        {
            int loggedInUserId = 1;
            // The code here is for loading data from the database to the web app
            var allPosts = await _postsService.GetAllPostsAsync(loggedInUserId);
            //
            return View(allPosts); 
        }

        //Creating Post
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel post)
        {
            //Get the logged in user
            int loggedInUser = 1;

            //Create a new post
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ImageUrl = "",
                NumOfReports = 0,
                UserId = loggedInUser
            };

            await _postsService.CreatePostAsync(newPost, post.Image);
            await _trendingsService.ProcessTrendingsForNewPostAsync(post.Content);
            

            return RedirectToAction("Index");
        }

        //Post Like
        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeViewModel postLikeViewModel)
        {
            int loggedInUserId = 1;

            await _postsService.TogglePostLikeAsync(postLikeViewModel.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        // Post - Report
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportViewModel postReportViewModel)
        {
            int loggedInUserId = 1;

            await _postsService.ReportPostAsync(postReportViewModel.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        // Post - Set As Private
        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityViewModel postVisibilityViewModel)
        {
            int loggedInUserId = 1;

            await _postsService.TogglePostVisibilityAsync(postVisibilityViewModel.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        // Post Favorite/Bookmark
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteViewModel postFavoriteViewModel)
        {
            int loggedInUserId = 1;
                
            await _postsService.TogglePostFavoriteAsync(postFavoriteViewModel.PostId, loggedInUserId);

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

            await _postsService.AddPostCommentAsync(newComment);

            return RedirectToAction("Index");
        }

        // Remove Comment/Reply
        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentViewModel removeCommentViewModel)
        {
            await _postsService.RemovePostCommentAsync(removeCommentViewModel.CommentId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostRemove(PostRemoveViewModel postRemoveViewModel)
        {

            var postRemoved = await _postsService.RemovePostAsync(postRemoveViewModel.PostId);
            await _trendingsService.ProcessTrendingsForRemovedPostAsync(postRemoved.Content);


            return RedirectToAction("Index");
        }
    }
}
