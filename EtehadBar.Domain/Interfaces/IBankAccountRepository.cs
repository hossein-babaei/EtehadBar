using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<BankAccount> Get(long id);
        Task<BankAccount> Get(string rowId);
        IQueryable<BankAccount> Query();
        void Update(BankAccount obj);
        void Create(BankAccount obj);
        void Delete(BankAccount obj);
        Task<int> Save();
    }
}
