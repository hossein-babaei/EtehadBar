using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class VehicleBankAccountRepository : IVehicleBankAccountRepository
    {
        private readonly ApplicationDbContext db;

        public VehicleBankAccountRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task Create(VehicleBankAccount obj)
        {
            await db.AddAsync(obj);
        }

        public void Delete(VehicleBankAccount obj)
        {
            db.Remove(obj);
        }

        public async Task<VehicleBankAccount> Get(long id)
        {
            return await db.VehicleBankAccount.FindAsync(id);
        }

        public async Task<VehicleBankAccount> Get(string rowId)
        {
            return await db.VehicleBankAccount.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public IQueryable<VehicleBankAccount> Query()
        {
            return db.VehicleBankAccount.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(VehicleBankAccount obj)
        {
            db.Update(obj);
        }
    }
}
