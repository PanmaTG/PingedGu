using Microsoft.AspNetCore.Mvc;
using PingedGu.Data;
using PingedGu.Data.Models;
using PingedGu.ViewModels.Stories;

namespace PingedGu.Controllers
{
    public class StoriesController : Controller
    {
        private readonly WebAppDbContext _context;
        public StoriesController(WebAppDbContext context) 
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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
            if (storyViewModel.Image != null && storyViewModel.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (storyViewModel.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/stories");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(storyViewModel.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await storyViewModel.Image.CopyToAsync(stream);
                    }

                    //Set the ImageUrl property of the new post to the relative path of the saved image
                    newStory.ImageUrl = "/images/stories/" + fileName;
                }
            }

            await _context.Stories.AddAsync(newStory);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
