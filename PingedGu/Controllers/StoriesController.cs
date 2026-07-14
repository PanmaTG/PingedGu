using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;
using PingedGu.Data.Helpers.Enums;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Stories;

namespace PingedGu.Controllers
{
    [Authorize]
    public class StoriesController : Controller
    {
        private readonly IStoriesService _storiesService;
        private readonly IFilesService _filesService;
        public StoriesController(IStoriesService storiesService, IFilesService filesService) 
        {
            _storiesService = storiesService;
            _filesService = filesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryViewModel storyViewModel)
        {
            int loggedInUserId = 1;

            var imageUploadPath = await _filesService.UploadImageAsync(storyViewModel.Image, ImageFileType.StoryImage);

            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                ImageUrl = imageUploadPath,
                UserId = loggedInUserId,
            };

            //For checking and saving of image

            await _storiesService.CreateStoryAsync(newStory);

            return RedirectToAction("Index", "Home");
        }
    }
}
