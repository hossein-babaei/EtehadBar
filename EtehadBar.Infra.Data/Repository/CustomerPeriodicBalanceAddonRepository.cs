using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class CustomerPeriodicBalanceAddonRepository : ICustomerPeriodicBalanceAddonRepository
    {
        private readonly ApplicationDbContext db;

        public CustomerPeriodicBalanceAddonRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(CustomerPeriodicBalanceAddon obj)
        {
            db.Add(obj);
        }

        public void Delete(CustomerPeriodicBalanceAddon obj)
        {
            db.Remove(obj);
        }

        public async Task<CustomerPeriodicBalanceAddon> Get(long id)
        {
            return await db.CustomerPeriodicBalanceAddon.FindAsync(id);
        }

        public async Task<CustomerPeriodicBalanceAddon> Get(string rowId)
        {
            return await db.CustomerPeriodicBalanceAddon.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public IQueryable<CustomerPeriodicBalanceAddon> Query()
        {
            return db.CustomerPeriodicBalanceAddon.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(CustomerPeriodicBalanceAddon obj)
        {
            db.Update(obj);
        }
    }
}
