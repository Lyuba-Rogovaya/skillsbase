using SkillsBase.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Interfaces
{
    public interface IReportService: IDisposable
    {
        /// <summary>
        /// Provides statistical data based user skills and domains and aggregated information about other users. 
        /// </summary>
        Task<UserStatistics> GetUserStatistics(string userId);
    }
}
