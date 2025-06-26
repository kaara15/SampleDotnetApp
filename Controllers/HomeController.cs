using Microsoft.AspNetCore.Mvc;

namespace SampleDotnetApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Account()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Call()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                ViewData["CurrentUserEmail"] = User.Identity.Name;
                return View();
            }
            return RedirectToAction("Login", "Account");
        }
    }
}