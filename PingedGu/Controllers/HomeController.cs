using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PingedGu.Controllers.Base;
using PingedGu.Data.Helpers.Enums;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.Hubs;
using PingedGu.ViewModels.Timeline;

namespace PingedGu.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        //1. The code here is for loading data from the database to the web app

        private readonly ILogger<HomeController> _logger;
        private readonly IPostsService _postsService;
        private readonly ITrendingsService _trendingsService;
        private readonly IFilesService _filesService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationsService _notificationsService;

        //Constructor
        public HomeController(ILogger<HomeController> logger,
            IPostsService postsService,
            ITrendingsService trendingsService,
            IFilesService filesService,
            IHubContext<NotificationHub> hubContext,
            INotificationsService notificationsService)
        {
            _logger = logger;
            _postsService = postsService;
            _trendingsService = trendingsService;
            _filesService = filesService;
            _hubContext = hubContext;
            _notificationsService = notificationsService;
        }

        //-------------------

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            // The code here is for loading data from the database to the web app
            var allPosts = await _postsService.GetAllPostsAsync(loggedInUserId.Value);
            //
            return View(allPosts);
        }

        public async Task<IActionResult> Details(int postId)
        {
            var post = await _postsService.GetPostByIdAsync(postId);
            if (post == null) return RedirectToAction("Index");
            return View(post);
        }

        //Creating Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostViewModel post)
        {
            //Get the logged in user
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var imageUploadPath = await _filesService.UploadImageAsync(post.Image, ImageFileType.PostImage);

            //Create a new post
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ImageUrl = imageUploadPath,
                NumOfReports = 0,
                UserId = loggedInUserId.Value
            };

            await _postsService.CreatePostAsync(newPost);
            await _trendingsService.ProcessTrendingsForNewPostAsync(post.Content);

            var createdPost = await _postsService.GetPostByIdAsync(newPost.Id);
            return PartialView("Timeline/_Post", createdPost);
        }

        //Post Like
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeViewModel postLikeViewModel)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToLogin();

            await _postsService.TogglePostLikeAsync(postLikeViewModel.PostId, userId.Value);

            var post = await _postsService.GetPostByIdAsync(postLikeViewModel.PostId);

            var notificationNumber = await _notificationsService.GetUnreadNotificationsCountAsync(userId.Value);
            await _hubContext.Clients.User(post.UserId.ToString())
                .SendAsync("ReceiveNotification", notificationNumber);

            return PartialView("Timeline/_Post", post);
        }

        // Post - Report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostReport(PostReportViewModel postReportViewModel)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            await _postsService.ReportPostAsync(postReportViewModel.PostId, loggedInUserId.Value);

            var post = await _postsService.GetPostByIdAsync(postReportViewModel.PostId);
            return PartialView("Timeline/_Post", post);
        }

        // Post - Set As Private
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityViewModel postVisibilityViewModel)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            await _postsService.TogglePostVisibilityAsync(postVisibilityViewModel.PostId, loggedInUserId.Value);

            var post = await _postsService.GetPostByIdAsync(postVisibilityViewModel.PostId);
            return PartialView("Timeline/_Post", post);
        }

        // Post Favorite/Bookmark
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteViewModel postFavoriteViewModel)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();
            await _postsService.TogglePostFavoriteAsync(postFavoriteViewModel.PostId, loggedInUserId.Value);

            var post = await _postsService.GetPostByIdAsync(postFavoriteViewModel.PostId);
            return PartialView("Timeline/_Post", post);
        }

        // Post Comment/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostComment(PostCommentViewModel postCommentViewModel)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var newComment = new Comment()
            {
                UserId = loggedInUserId.Value,
                PostId = postCommentViewModel.PostId,
                Content = postCommentViewModel.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _postsService.AddPostCommentAsync(newComment);

            var post = await _postsService.GetPostByIdAsync(postCommentViewModel.PostId);
            return PartialView("Timeline/_Post", post);
        }

        // Remove Comment/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePostComment(RemoveCommentViewModel removeCommentViewModel)
        {
            await _postsService.RemovePostCommentAsync(removeCommentViewModel.CommentId);

            var post = await _postsService.GetPostByIdAsync(removeCommentViewModel.PostId);
            return PartialView("Timeline/_Post", post);
        }

        // Remove/Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostRemove(PostRemoveViewModel postRemoveViewModel)
        {

            var postRemoved = await _postsService.RemovePostAsync(postRemoveViewModel.PostId);
            await _trendingsService.ProcessTrendingsForRemovedPostAsync(postRemoved.Content);

            return Ok();
        }
    }
}
