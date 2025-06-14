using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleDotnetApp.Controllers
{
    [Authorize]
    public class SampleMVCController : Controller
    {
        public IActionResult SampleMVCPage()
        {
            return View();
        }
    }
}