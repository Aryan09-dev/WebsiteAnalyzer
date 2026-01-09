using Microsoft.EntityFrameworkCore;
using System;
using WebsiteAnalyzer.Domain.Entities;

namespace WebsiteAnalyzer.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<WebsiteScan> Website_Scans { get; set; }
        public DbSet<ScanPage> Scan_Pages { get; set; }
        public DbSet<AutomatedIssue> Automated_Issues { get; set; }
        public DbSet<ManualBug> Manual_Bugs { get; set; }
        public DbSet<SecurityHeader> Security_Headers { get; set; }
        public DbSet<PerformanceMetric> Performance_Metrics { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<AIRecommendation> AI_Recommendations { get; set; }
    }
}
