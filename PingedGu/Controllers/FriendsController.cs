using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Services;

namespace PingedGu.Controllers
{
    public class FriendsController : Controller
    {
        public readonly IFriendsService _friendsService;

        public FriendsController(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
