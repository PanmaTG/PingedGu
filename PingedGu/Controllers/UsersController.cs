using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingedGu.Controllers.Base;
using PingedGu.Data.Helpers.Constants;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Users;

namespace PingedGu.Controllers
{
    [Authorize(Roles = AppRoles.User)]
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<User> _userManager;
        private readonly IFriendsService _friendsService;

        public UsersController(IUsersService usersService, UserManager<User> userManager, IFriendsService friendsService)
        {
            _usersService = usersService;
            _userManager = userManager;
            _friendsService = friendsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userPosts = await _usersService.GetUserPosts(userId);
            var friends = await _friendsService.GetFriendsWithCountAsync(userId);

            var userProfileViewModel = new GetUserProfileViewModel()
            {
                User = user,
                Posts = userPosts,
                Friends = friends
            };

            return View(userProfileViewModel);
        }
    }
}
