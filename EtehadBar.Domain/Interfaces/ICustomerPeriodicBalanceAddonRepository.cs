using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ICustomerPeriodicBalanceAddonRepository
    {
        Task<CustomerPeriodicBalanceAddon> Get(long id);
        Task<CustomerPeriodicBalanceAddon> Get(string rowId);
        IQueryable<CustomerPeriodicBalanceAddon> Query();
        void Update(CustomerPeriodicBalanceAddon obj);
        void Create(CustomerPeriodicBalanceAddon obj);
        void Delete(CustomerPeriodicBalanceAddon obj);
        Task<int> Save();
    }
}
