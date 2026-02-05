using Microsoft.AspNetCore.Mvc;

namespace WebsiteAnalyzer.API.Controllers
{
    public class SecurityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
