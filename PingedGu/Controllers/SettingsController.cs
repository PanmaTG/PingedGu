using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingedGu.Controllers.Base;
using PingedGu.Data.Helpers.Constants;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Settings;
using System.Security.Claims;

namespace PingedGu.Controllers
{
    [Authorize(Roles = $"{AppRoles.User}, {AppRoles.Admin}")] 
    public class SettingsController : BaseController
    {
        private readonly IUsersService _usersService;
        private readonly IFilesService _filesService;
        private readonly UserManager<User> _userManager;

        public SettingsController(IUsersService usersService, IFilesService filesService, UserManager<User> userManager)
        {
            _usersService = usersService;
            _filesService = filesService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);

            return View(loggedInUser);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureViewModel profilePictureViewModel)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null) return RedirectToLogin();

            var uploadedProfilePictureUrl = await _filesService
                .UploadImageAsync(profilePictureViewModel.ProfilePictureImage, Data.Helpers.Enums.ImageFileType.ProfilePicture);

            await _usersService.UpdateUserProfilePicture(loggedInUserId.Value, uploadedProfilePictureUrl);

            return RedirectToAction("Index");
        }
    }
}
