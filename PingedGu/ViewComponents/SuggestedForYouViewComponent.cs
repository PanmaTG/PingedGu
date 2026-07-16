using Microsoft.AspNetCore.Mvc;

namespace PingedGu.ViewComponents
{
    public class SuggestedForYouViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
