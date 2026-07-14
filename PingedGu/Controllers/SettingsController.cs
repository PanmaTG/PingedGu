using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Settings;

namespace PingedGu.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IFilesService _filesService;

        public SettingsController(IUsersService usersService, IFilesService filesService)
        {
            _usersService = usersService;
            _filesService = filesService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = 1;
            var userDb = await _usersService.GetUser(loggedInUserId);
            return View(userDb);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureViewModel profilePictureViewModel)
        {
            var loggedInUser = 1;
            var uploadedProfilePictureUrl = await _filesService
                .UploadImageAsync(profilePictureViewModel.ProfilePictureImage, Data.Helpers.Enums.ImageFileType.ProfilePicture);

            await _usersService.UpdateUserProfilePicture(loggedInUser, uploadedProfilePictureUrl);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel profileViewModel)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel updatePasswordViewModel)
        {
            return RedirectToAction("Index");
        }
    }
}
