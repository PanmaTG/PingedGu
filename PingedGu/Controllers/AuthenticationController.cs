using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingedGu.Data.Helpers.Constants;
using PingedGu.Data.Models;
using PingedGu.ViewModels.Authentication;
using PingedGu.ViewModels.Settings;
using System.Security.Claims;

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
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var existingUser = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (existingUser == null) 
            {
                ModelState.AddModelError("", "Invalid Email or Password. Please try again");
                return View(loginViewModel);
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(existingUser);

            if(!existingUserClaims.Any(c => c.Type == CustomClaim.FullName))
            {
                await _userManager.AddClaimAsync(existingUser, new Claim(CustomClaim.FullName, existingUser.FullName));
            }

            var result = await _signInManager.PasswordSignInAsync(existingUser.UserName, loginViewModel.Password, false, false);

            if (result.Succeeded)
            {
                if (User.IsInRole(AppRoles.Admin))
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
                

            var newUser = new User()
            {
                FullName = $"{registerViewModel.FirstName} {registerViewModel.LastName}",
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email
            };

            var existingUser = await _userManager.FindByEmailAsync(registerViewModel.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(registerViewModel);
            }

            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);

                await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.FullName, newUser.FullName));
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }


            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel updatePasswordViewModel)
        {
            if (updatePasswordViewModel.NewPassword != updatePasswordViewModel.ConfirmPassword)
            {
                TempData["PasswordError"] = "Passwords do not match!";
                TempData["ActiveTab"] = "Password";

                return RedirectToAction("Index", "Settings");
            }

            var loggedInUser = await _userManager.GetUserAsync(User);
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(loggedInUser, updatePasswordViewModel.CurrentPassword);

            if (!isCurrentPasswordValid)
            {
                TempData["PasswordError"] = "Passwords do not match!";
                TempData["ActiveTab"] = "Password";

                return RedirectToAction("Index", "Settings");
            }

            var result = await _userManager.ChangePasswordAsync(loggedInUser, updatePasswordViewModel.CurrentPassword, updatePasswordViewModel.NewPassword);

            if(result.Succeeded)
            {
                TempData["PasswordSuccess"] = "Password updated successfully";
                TempData["ActiveTab"] = "Password";

                await _signInManager.RefreshSignInAsync(loggedInUser);
            }

            return RedirectToAction("Index", "Settings");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel profileViewModel)
        {
            var loggeddInUser = await _userManager.GetUserAsync(User);
            if (loggeddInUser == null) 
            {
                return RedirectToAction("Login");
            }

            loggeddInUser.FullName = profileViewModel.FullName;
            loggeddInUser.UserName = profileViewModel.UserName;
            loggeddInUser.Bio = profileViewModel.Bio;

            var result = await _userManager.UpdateAsync(loggeddInUser);
            if (!result.Succeeded)
            {
                TempData["UserProfileError"] = "User profile could not be updated";
                TempData["ActiveTab"] = "Profile";
            }

            await _signInManager.RefreshSignInAsync(loggeddInUser);
            return RedirectToAction("Index", "Settings");
        }

        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Authentication");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback()
         {
            var info = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if(info == null)
            {
                return RedirectToAction("Login");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var newUser = new User()
                {
                    Email = email,
                    UserName = email,
                    FullName = info.Principal.FindFirstValue(ClaimTypes.Name),
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, AppRoles.User);
                    await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.FullName, newUser.FullName));
                    await _signInManager.SignInAsync(newUser, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
