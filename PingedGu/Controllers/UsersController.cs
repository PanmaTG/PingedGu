using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingedGu.Controllers.Base;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Users;

namespace PingedGu.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<User> _userManager;

        public UsersController(IUsersService usersService, UserManager<User> userManager)
        {
            _usersService = usersService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userPosts = await _usersService.GetUserPosts(userId);

            var userProfileViewModel = new GetUserProfileViewModel()
            {
                User = user,
                Posts = userPosts
            };

            return View(userProfileViewModel);
        }
    }
}
