using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IAccountBookRepository
    {
        Task<AccountBook> Get(long id);
        Task<AccountBook> Get(string rowId);
        IQueryable<AccountBook> AccountBooks();
        void Update(AccountBook obj);
        void Create(AccountBook obj);
        void Delete(AccountBook obj);
        Task UpdateAmount(long accountBookId);
        Task<int> Save();
    }
}
