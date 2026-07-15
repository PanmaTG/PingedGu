using Microsoft.AspNetCore.Mvc;
using PingedGu.Controllers.Base;

namespace PingedGu.Controllers
{
    public class UsersController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(string userId)
        {
            return View();
        }
    }
}
