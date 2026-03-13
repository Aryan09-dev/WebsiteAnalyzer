using System;
using System.Collections.Generic;
namespace WebsiteAnalyzer.Application.DTOs
{
    public class ScanResultDto
    {
        public int Scan_Id { get; set; }
        public string Website_Url { get; set; }

        public int Performance_Score { get; set; }
        public int Security_Score { get; set; }
        public int Code_Quality_Score { get; set; }

        public List<IssueDto> Bugs { get; set; }
        public List<IssueDto> CodeQuality { get; set; }

        public List<PerformanceMetricDto> Performance { get; set; }

        public List<SecurityHeaderDto> Security { get; set; }
    }
}
