using Microsoft.AspNetCore.Mvc;

namespace SampleDotnetApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}