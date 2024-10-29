using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class ShippingFeeGroupRepository : IShippingFeeGroupRepository
    {
        private readonly ApplicationDbContext db;
        public ShippingFeeGroupRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<ShippingFeeGroup> Query()
        {
            return db.ShippingFeeGroup.AsQueryable();
        }

        public void Create(ShippingFeeGroup obj)
        {
            db.Add(obj);
        }

        public void Delete(ShippingFeeGroup obj)
        {
            db.Remove(obj);
        }

        public async Task<ShippingFeeGroup> Get(long id)
        {
            return await db.ShippingFeeGroup.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(ShippingFeeGroup obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<ShippingFeeGroup> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }

        public void UpdateLoadFactors(List<LoadFactor> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }
    }
}
