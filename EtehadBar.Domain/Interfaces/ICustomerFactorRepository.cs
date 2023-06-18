using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ICustomerFactorRepository
    {
        Task<CustomerFactor> Get(long id);
        Task<CustomerFactor> Get(string rowId);
        IQueryable<CustomerFactor> Query();
        void Update(CustomerFactor obj);
        void Create(CustomerFactor obj);
        void Delete(CustomerFactor obj);
        Task<int> Save();
    }
}
