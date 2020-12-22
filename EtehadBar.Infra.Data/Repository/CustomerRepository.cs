using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext db;
        public CustomerRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Customer obj)
        {
            db.Add(obj);
        }

        public async Task<List<Customer>> GetAll()
        {
            return await db.Customer.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<Customer> Get(int id)
        {
            return await db.Customer.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Customer obj)
        {
            db.Update(obj);
        }

        public async Task<CustomerIncome> GetIncome(int id)
        {
            return await db.CustomerIncome.FindAsync(id);
        }

        public IQueryable<CustomerIncome> CustomerIncomes()
        {
            return db.CustomerIncome.AsQueryable();
        }

        public void Update(CustomerIncome obj)
        {
            db.Update(obj);
        }

        public void Create(CustomerIncome obj)
        {
            db.Add(obj);
        }

        public void Delete(CustomerIncome obj)
        {
            db.Remove(obj);
        }
    }
}
