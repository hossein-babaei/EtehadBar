using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class CustomerPeriodicBalanceSummaryRepository : ICustomerPeriodicBalanceSummaryRepository
    {
        private readonly ApplicationDbContext db;

        public CustomerPeriodicBalanceSummaryRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(CustomerPeriodicBalanceSummary obj)
        {
            db.Add(obj);
        }

        public void Delete(CustomerPeriodicBalanceSummary obj)
        {
            db.Remove(obj);
        }

        public async Task<CustomerPeriodicBalanceSummary> Get(long id)
        {
            return await db.CustomerPeriodicBalanceSummary.FindAsync(id);
        }

        public async Task<CustomerPeriodicBalanceSummary> Get(string rowId)
        {
            return await db.CustomerPeriodicBalanceSummary.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public IQueryable<CustomerPeriodicBalanceSummary> Query()
        {
            return db.CustomerPeriodicBalanceSummary.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(CustomerPeriodicBalanceSummary obj)
        {
            db.Update(obj);
        }
    }
}
