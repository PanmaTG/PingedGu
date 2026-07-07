using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;
using PingedGu.Data.Models;
using PingedGu.Data.Services;
using PingedGu.ViewModels.Stories;

namespace PingedGu.Controllers
{
    public class StoriesController : Controller
    {
        private readonly IStoriesService _storiesService;
        public StoriesController(IStoriesService storiesService) 
        {
            _storiesService = storiesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryViewModel storyViewModel)
        {
            int loggedInUserId = 1;

            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                UserId = loggedInUserId,
            };

            //For checking and saving of image

            await _storiesService.CreateStoryAsync(newStory, storyViewModel.Image);

            return RedirectToAction("Index", "Home");
        }
    }
}
