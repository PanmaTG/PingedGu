using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Helpers.Constants;
using PingedGu.Data.Services;
using System.Security.Claims;

namespace PingedGu.Controllers
{
    [Authorize(Roles = AppRoles.User)]
    public class FavoritesController : Controller
    {
        private readonly IPostsService _postsService;

        public FavoritesController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myFavoritePosts = await _postsService.GetAllFavoritedPostsAsync(int.Parse(loggedInUserId));

            return View(myFavoritePosts);
        }
    }
}
