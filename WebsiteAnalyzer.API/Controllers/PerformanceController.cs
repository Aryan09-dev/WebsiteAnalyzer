using Microsoft.AspNetCore.Mvc;

namespace WebsiteAnalyzer.API.Controllers
{
    public class PerformanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
