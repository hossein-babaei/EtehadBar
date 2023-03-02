using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IBankAccountBookRepository
    {
        Task<BankAccountBook> Get(long id);
        Task<BankAccountBook> Get(string rowId);
        IQueryable<BankAccountBook> Query();
        void Update(BankAccountBook obj);
        void Create(BankAccountBook obj);
        void Delete(BankAccountBook obj);
        Task<int> Save();
    }
}
