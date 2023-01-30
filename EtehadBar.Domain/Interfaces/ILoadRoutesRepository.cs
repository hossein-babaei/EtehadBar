using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ILoadRoutesRepository
    {
        Task<bool> CheckNameExist(string title);
        Task<LoadRoutes> Get(long id);
        IQueryable<LoadRoutes> LoadRoutes();
        void Update(LoadRoutes obj);
        void UpdateRange(List<LoadRoutes> list);
        void Create(LoadRoutes obj);
        void Delete(LoadRoutes obj);
        Task<int> Save();
    }
}
