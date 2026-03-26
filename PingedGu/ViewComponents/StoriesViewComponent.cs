using Microsoft.AspNetCore.Mvc;

namespace PingedGu.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> Invoke()
        {
            return View();
        }
    }
}
