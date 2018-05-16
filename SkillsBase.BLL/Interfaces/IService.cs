using SkillsBase.BLL.DTO;
using SkillsBase.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillsBase.BLL.Interfaces
{
    public interface IService<T> : IDisposable
    {
        Task SaveAsync(T dto);
        Task DeleteAsync(T dto);
        Task<IEnumerable<T>> ListAllAsync();
        Task<GridSearchRestltDTO<T>> FindByCriteriaAsync(string search, int start, int length);
        Task<T> FindAsync(int id);
        Task<T> FindAsync(string id);
    }
}
