using Microsoft.AspNetCore.Mvc;
using WebsiteAnalyzer.Application.Interfaces;

namespace WebsiteAnalyzer.API.Controllers
{
    [ApiController]
    [Route("api/scan")]
    public class WebsiteScanController : ControllerBase
    {
        private readonly IWebsiteScanRepository _scanRepository;

        public WebsiteScanController(IWebsiteScanRepository scanRepository)
        {
            _scanRepository = scanRepository;
        }
    }
}
