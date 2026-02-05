using Microsoft.AspNetCore.Mvc;

namespace WebsiteAnalyzer.API.Controllers
{
    public class IssueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
