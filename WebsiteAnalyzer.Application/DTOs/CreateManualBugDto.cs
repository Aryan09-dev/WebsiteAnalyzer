using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteAnalyzer.Application.DTOs
{
    public class CreateManualBugDto
    {
        public int Scan_Id { get; set; }
        public string Page_Url { get; set; } = null!;
        public string Bug_Title { get; set; } = null!;
        public string? Bug_Description { get; set; }
        public string Severity { get; set; } = null!;
        public string? Screenshot_Path { get; set; }
        public int Reported_By { get; set; }
    }
}
