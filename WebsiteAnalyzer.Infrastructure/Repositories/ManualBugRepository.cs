using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Domain.Entities;
using WebsiteAnalyzer.Infrastructure.Data;

namespace WebsiteAnalyzer.Infrastructure.Repositories
{
    public class ManualBugRepository : IManualBugRepository
    {
        private readonly ApplicationDbContext _context;

        public ManualBugRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ManualBug> CreateAsync(ManualBug bug)
        {
            bug.Created_On = DateTime.UtcNow;
            bug.Is_Active = true;
            bug.Is_Deleted = false;

            await _context.Manual_Bugs.AddAsync(bug);
            await _context.SaveChangesAsync();

            return bug;
        }

        public async Task<List<ManualBug>> GetByUserIdAsync(int userId)
        {
            return await _context.Manual_Bugs
                .Where(b => b.Reported_By == userId && b.Is_Active && !b.Is_Deleted)
                .OrderByDescending(b => b.Created_On)
                .ToListAsync();
        }

        public async Task<ManualBug?> UpdateAsync(int id, ManualBug updatedBug)
        {
            var existing = await _context.Manual_Bugs.FindAsync(id);

            if (existing == null || existing.Is_Deleted)
                return null;

            existing.Bug_Title = updatedBug.Bug_Title;
            existing.Bug_Description = updatedBug.Bug_Description;
            existing.Severity = updatedBug.Severity;
            existing.Page_Url = updatedBug.Page_Url;
            existing.Modified_On = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
