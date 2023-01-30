using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class ShippingFeeLoadTypeRepository : IShippingFeeLoadTypeRepository
    {
        private readonly ApplicationDbContext db;

        public ShippingFeeLoadTypeRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<bool> CheckNameExist(string name)
        {
            return await db.ShippingFeeLoadType.AsNoTracking().AnyAsync(a => a.Name.Equals(name));
        }

        public void Create(ShippingFeeLoadType obj)
        {
            db.Add(obj);
        }

        public void Delete(ShippingFeeLoadType obj)
        {
            db.Remove(obj);
        }

        public async Task<ShippingFeeLoadType> Get(long id)
        {
            return await db.ShippingFeeLoadType.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public IQueryable<ShippingFeeLoadType> ShippingFeeLoadTypes()
        {
            return db.ShippingFeeLoadType.AsQueryable();
        }

        public void Update(ShippingFeeLoadType obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<ShippingFeeLoadType> list)
        {
            db.UpdateRange(list);
        }
    }
}
