using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IChequeRepository
    {
        Task<Cheque> Get(long id);
        Task<Cheque> Get(string rowId);
        Task<EditChequeVM> GetEditData(long id);
        IQueryable<Cheque> Query();
        void Update(Cheque obj);
        void Create(Cheque obj);
        void Delete(Cheque obj);
        Task<int> Save();
    }
}
