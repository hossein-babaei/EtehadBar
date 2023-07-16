using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ICustomerPeriodicBalanceSummaryRepository
    {
        Task<CustomerPeriodicBalanceSummary> Get(long id);
        Task<CustomerPeriodicBalanceSummary> Get(string rowId);
        IQueryable<CustomerPeriodicBalanceSummary> Query();
        void Update(CustomerPeriodicBalanceSummary obj);
        void Create(CustomerPeriodicBalanceSummary obj);
        void Delete(CustomerPeriodicBalanceSummary obj);
        Task<int> Save();
    }
}
