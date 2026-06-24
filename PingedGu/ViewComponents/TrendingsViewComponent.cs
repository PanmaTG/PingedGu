using Microsoft.AspNetCore.Mvc;

namespace PingedGu.ViewComponents
{
    public class TrendingsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
