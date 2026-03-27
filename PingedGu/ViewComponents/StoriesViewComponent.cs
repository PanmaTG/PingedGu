using Microsoft.AspNetCore.Mvc;
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

        public async Task<IViewComponentResult> Invoke()
        {
            return View();
        }
    }
}
 