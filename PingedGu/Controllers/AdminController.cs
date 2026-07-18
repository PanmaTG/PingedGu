using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Services;

namespace PingedGu.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
