using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ILoadFactorNovinRepository
    {
        Task<LoadFactorNovin> Get(long id);
        Task<LoadFactorNovin> Get(string rowId);
        Task<EditLoadFactorNovinVM> GetEditData(long id);
        IQueryable<LoadFactorNovin> Query();
        void Update(LoadFactorNovin obj);
        void Create(LoadFactorNovin obj);
        void Delete(LoadFactorNovin obj);
        Task<int> Save();
    }
}
