using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;

namespace PingedGu.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        private readonly WebAppDbContext _context;
        public StoriesViewComponent(WebAppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _context.Stories.Include(s => s.User).ToListAsync();
            return View(allStories);
        }
    }
}
 