using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> Get(long id);
        Task<Bill> Get(string rowId);
        IQueryable<Bill> Query();
        void Update(Bill obj);
        void Create(Bill obj);
        void Delete(Bill obj);
        Task<int> Save();
    }
}
