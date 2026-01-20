using Microsoft.Extensions.Configuration;
using System.Text.Json;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Infrastructure.Data;

namespace WebsiteAnalyzer.Infrastructure.Repositories
{
    public class WebsiteScanRepository : IWebsiteScanRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WebsiteScanRepository(
            ApplicationDbContext context,
            HttpClient httpClient,
            IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<object> AnalyzeAsync(StartScanDto dto)
        {
            var apiKey = _config["PageSpeedInsights:ApiKey"];

            var url = $"https://pagespeedonline.googleapis.com/pagespeedonline/v5/runPagespeed" +
                     $"?url={Uri.EscapeDataString(dto.Website_Url)}" +
                     $"&key={apiKey}" +
                     $"&strategy=desktop";


            var response = await _httpClient.GetStringAsync(url);

            using JsonDocument json = JsonDocument.Parse(response);

            var lighthouse = json.RootElement
                .GetProperty("lighthouseResult")
                .GetProperty("categories");

            return new
            {
                Website = dto.Website_Url,
                Performance = lighthouse.GetProperty("performance").GetProperty("score").GetDouble() * 100,
                Accessibility = lighthouse.GetProperty("accessibility").GetProperty("score").GetDouble() * 100,
                BestPractices = lighthouse.GetProperty("best-practices").GetProperty("score").GetDouble() * 100,
                SEO = lighthouse.GetProperty("seo").GetProperty("score").GetDouble() * 100
            };
        }
    }
}
