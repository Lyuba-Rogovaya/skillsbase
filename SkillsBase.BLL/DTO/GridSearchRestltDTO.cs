using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.DTO
{
    public class GridSearchRestltDTO<T>
    {
        public int TotalResultsCount { get; set; }
        public int FilteredResultsCount { get; set; }
        public IEnumerable<T> Items;
    }
}
