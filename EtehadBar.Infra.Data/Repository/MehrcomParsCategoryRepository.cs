using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class MehrcomParsCategoryRepository : IMehrcomParsCategoryRepository
    {
        private readonly ApplicationDbContext db;

        public MehrcomParsCategoryRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<MehrcomParsCategory> Categories()
        {
            return db.MehrcomParsCategory.AsQueryable();
        }

        public void Create(MehrcomParsCategory obj)
        {
            db.Add(obj);
        }

        public void Delete(MehrcomParsCategory obj)
        {
            db.Remove(obj);
        }

        public async Task<MehrcomParsCategory> Get(long id)
        {
            return await db.MehrcomParsCategory.FindAsync(id);
        }

        public async Task<MehrcomParsCategory> Get(string id)
        {
            return await db.MehrcomParsCategory.FirstOrDefaultAsync(a => a.RowId.Equals(id));
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(MehrcomParsCategory obj)
        {
            db.Update(obj);
        }
    }
}
