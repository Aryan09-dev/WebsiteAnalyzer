using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteAnalyzer.Domain.Entities;

namespace WebsiteAnalyzer.Application.Interfaces
{
    public interface IManualBugRepository
    {
        Task<ManualBug> CreateAsync(ManualBug bug);
        Task<List<ManualBug>> GetByUserIdAsync(int userId);
    }
}
