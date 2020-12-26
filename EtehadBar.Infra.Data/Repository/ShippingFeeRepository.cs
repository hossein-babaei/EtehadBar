using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
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

        public async Task<ShippingFee> Get(string id)
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
    }
}
