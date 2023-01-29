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
    public class ShippingFeeRepository : IShippingFeeRepository
    {
        private readonly ApplicationDbContext db;
        public ShippingFeeRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<ShippingFee> ShippingFees()
        {
            return db.ShippingFee.AsQueryable();
        }

        public void Create(ShippingFee obj)
        {
            db.Add(obj);
        }

        public void Delete(ShippingFee obj)
        {
            db.Remove(obj);
        }

        public async Task<ShippingFee> Get(long id)
        {
            return await db.ShippingFee.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(ShippingFee obj)
        {
            db.Update(obj);
        }

        public async Task<List<LoadFactor>> GetLoadFactorsByContractId(long contractId, DateTime date)
        {
            return await db.LoadFactor.Where(a => a.ContractId.Equals(contractId) && a.Date >= date).ToListAsync();
        }

        public void UpdateRange(List<ShippingFee> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }

        public void UpdateLoadFactors(List<LoadFactor> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }
    }
}
