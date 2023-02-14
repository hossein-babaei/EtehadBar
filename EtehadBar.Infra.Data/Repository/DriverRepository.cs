using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ApplicationDbContext db;

        public DriverRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<Driver> Drivers()
        {
            return db.Driver.AsQueryable();
        }

        public void Create(Driver obj)
        {
            db.Add(obj);
        }

        public void Delete(Driver obj)
        {
            db.Remove(obj);
        }

        public async Task<Driver> Get(long id)
        {
            return await db.Driver.FindAsync(id);
        }

        public async Task<Driver> Get(string id)
        {
            return await db.Driver.FirstOrDefaultAsync(a => a.RowId.Equals(id));
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Driver obj)
        {
            db.Update(obj);
        }
    }
}
