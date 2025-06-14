using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using SampleDotnetApp.Models.DataModels;
using SampleDotnetApp.Models.ViewModels;
using SampleDotnetApp.Services.Interfaces;
using SampleDotnetApp.Utilities.Interfaces;

namespace SampleDotnetApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDataCacheService<DataCacheDataModel> dataCacheService;
        public AccountController(ICacheFactory<DataCacheDataModel> cacheFactory)
        {
            dataCacheService = cacheFactory.GetDefaultCache();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SampleMVCPage", "SampleMVC");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SampleMVCPage", "SampleMVC");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                DataCacheDataModel dataCacheDataModel = new()
                {
                    Email = registerViewModel.Email,
                    FullName = registerViewModel.FullName,
                    HashedPassword = registerViewModel.HashedPassword
                };
                if(dataCacheService.AddToCache(dataCacheDataModel))
                {
                    ModelState.AddModelError(string.Empty, "Registration successful. Please login.");
                    TempData["AlertMessage"] = "Registration successful. Please login.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Registration failed. Email already exists.");
                    TempData["AlertMessage"] = "Registration failed. Email already exists.";
                    return View();
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid registration attempt.");
            TempData["AlertMessage"] = "Invalid registration attempt.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(ModelState.IsValid)
            {
                var email = loginViewModel.Email;
                var hashedPassword = loginViewModel.HashedPassword;
            
                if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(hashedPassword))
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    TempData["AlertMessage"] = "Invalid login attempt.";
                    return View();
                }
                DataCacheDataModel dataCacheDataModel = dataCacheService.GetFromCache(email);
                if(dataCacheDataModel == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                    TempData["AlertMessage"] = "User not found.";
                    return View();
                }
                if(dataCacheDataModel.Email != email)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email address.");
                    TempData["AlertMessage"] = "Invalid email address.";
                    return View();
                }
                if(dataCacheDataModel.HashedPassword != hashedPassword)
                {
                    ModelState.AddModelError(string.Empty, "Invalid password.");
                    TempData["AlertMessage"] = "Invalid password.";
                    return View();
                }

                IEnumerable<Claim> claims =
                [
                    new(ClaimTypes.Name, email),
                    new(ClaimTypes.Role, "admin")
                ];
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                            new ClaimsPrincipal(claimsIdentity), 
                                            new AuthenticationProperties() {
                                                IsPersistent = true,
                                                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                                            });
                TempData["AlertMessage"] = "Login successful.";
                return RedirectToAction("SampleMVCPage", "SampleMVC");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            TempData["AlertMessage"] = "Invalid login attempt.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["AlertMessage"] = "Logout successful.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var props = new AuthenticationProperties()
            {
                RedirectUri = returnUrl
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if(!result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            TempData["AlertMessage"] = "Login successful.";
            return RedirectToAction("SampleMVCPage", "SampleMVC");
        }
    }
}