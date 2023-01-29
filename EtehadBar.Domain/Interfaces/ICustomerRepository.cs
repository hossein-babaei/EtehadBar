using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> Get(long id);
        IQueryable<Customer> Customers();
        Task<List<Customer>> GetAll();
        Task<List<Customer>> GetAllActive();
        void Update(Customer obj);
        void Create(Customer obj);

        Task<CustomerIncome> GetIncome(long id);
        IQueryable<CustomerIncome> CustomerIncomes();
        void Update(CustomerIncome obj);
        void Create(CustomerIncome obj);
        void Delete(CustomerIncome obj);

        Task<int> Save();
    }
}
