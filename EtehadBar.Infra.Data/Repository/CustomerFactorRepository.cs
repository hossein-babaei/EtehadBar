using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class CustomerFactorRepository : ICustomerFactorRepository
    {
        private readonly ApplicationDbContext db;

        public CustomerFactorRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(CustomerFactor obj)
        {
            db.Add(obj);
        }

        public void Delete(CustomerFactor obj)
        {
            db.Remove(obj);
        }

        public async Task<CustomerFactor> Get(long id)
        {
            return await db.CustomerFactor.FindAsync(id);
        }

        public async Task<CustomerFactor> Get(string rowId)
        {
            return await db.CustomerFactor.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public IQueryable<CustomerFactor> Query()
        {
            return db.CustomerFactor.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(CustomerFactor obj)
        {
            db.Update(obj);
        }
    }
}
