using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Helpers.Constants;
using PingedGu.Data.Models;
using PingedGu.ViewModels.Authentication;

namespace PingedGu.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var newUser = new User()
            {
                FullName = $"{registerViewModel.FirstName} {registerViewModel.LastName}",
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email
            };

            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);

                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }   


            return View();
        }
    }
}
