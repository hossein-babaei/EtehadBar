using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IFreeLoadFactorRepository
    {
        Task<FreeLoadFactor> Get(long id);
        Task<EditFreeLoadFactorVM> GetEditData(long id);
        IQueryable<FreeLoadFactor> Query();
        void Update(FreeLoadFactor obj);
        void Create(FreeLoadFactor obj);
        void Delete(FreeLoadFactor obj);
        Task<int> Save();
    }
}
