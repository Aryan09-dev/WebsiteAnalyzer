using Microsoft.AspNetCore.Mvc;

namespace WebsiteAnalyzer.API.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
