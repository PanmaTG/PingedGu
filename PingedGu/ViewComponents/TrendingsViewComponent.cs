using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingedGu.Data;

namespace PingedGu.ViewComponents
{
    public class TrendingsViewComponent : ViewComponent
    {
        private readonly WebAppDbContext _context;
        public TrendingsViewComponent(WebAppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var oneWeekAgoNow = DateTime.UtcNow.AddDays(-7);

            var top3Trendings = await _context.Trendings
                .Where(h => h.DateCreated > oneWeekAgoNow)
                .OrderByDescending(n => n.Count)
                .Take(6)
                .ToListAsync();

            return View(top3Trendings);
        }
    }
}
