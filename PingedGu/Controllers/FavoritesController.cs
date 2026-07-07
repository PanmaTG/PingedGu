using Microsoft.AspNetCore.Mvc;

namespace PingedGu.Controllers
{
    public class FavoritesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
