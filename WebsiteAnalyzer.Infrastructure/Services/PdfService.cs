using WebsiteAnalyzer.Application.DTOs;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;

namespace WebsiteAnalyzer.Infrastructure.Services
{
    public class PdfService
    {
        public byte[] GenerateReport(ScanResultDto result)
        {
            using var memoryStream = new MemoryStream();

            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph("Website Scan Report").SetFontSize(20));

            document.Add(new Paragraph($"Website: {result.Website_Url}"));
            document.Add(new Paragraph($"Performance Score: {result.Performance_Score}"));
            document.Add(new Paragraph($"Security Score: {result.Security_Score}"));
            document.Add(new Paragraph($"Code Quality Score: {result.Code_Quality_Score}"));

            document.Add(new Paragraph("\nDetected Bugs"));

            foreach (var bug in result.Bugs)
            {
                document.Add(new Paragraph($"• {bug.Title} - {bug.Severity}"));
            }

            document.Add(new Paragraph("\nSecurity Headers"));

            foreach (var sec in result.Security)
            {
                document.Add(new Paragraph($"• {sec.Header_Name}: {sec.Status}"));
            }

            document.Close();

            return memoryStream.ToArray();
        }
    }
}
