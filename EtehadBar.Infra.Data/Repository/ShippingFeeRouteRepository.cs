using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class ShippingFeeRouteRepository : IShippingFeeRouteRepository
    {
        private readonly ApplicationDbContext db;
        public ShippingFeeRouteRepository(ApplicationDbContext context)
        {
            db = context;
        }
        public void Create(ShippingFeeRoute obj)
        {
            db.Add(obj);
        }

        public void Delete(ShippingFeeRoute obj)
        {
            db.Remove(obj);
        }

        public async Task<ShippingFeeRoute> Get(long id)
        {
            return await db.ShippingFeeRoute.FindAsync(id);
        }

        public async Task<ShippingFeeRoute> GetWithGroup(long id)
        {
            return await db.ShippingFeeRoute.AsNoTracking().Include(a => a.ShippingFeeGroup).FirstOrDefaultAsync(a => a.Id.Equals(id));
        }

        public async Task<ShippingFeeRoute> GetWithGroupAndLoadRoute(long id)
        {
            return await db.ShippingFeeRoute.AsNoTracking().Include(a => a.ShippingFeeGroup).ThenInclude(a => a.ShippingFeeLoadType).Include(a => a.Origin).Include(a => a.Destination).FirstOrDefaultAsync(a => a.Id.Equals(id));
        }

        public IQueryable<ShippingFeeRoute> Query()
        {
            return db.ShippingFeeRoute.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public async Task<List<ShippingFeeRouteWithPriceVM>> ShippingFeeRouteWithPrice(long contractId)
        {
            var data = await (from a in db.ShippingFeeRoute
                              join b in db.ShippingFeeGroup on a.ShippingFeeGroupId equals b.Id
                              join c in db.ShippingFeeLoadType on b.ShippingFeeLoadTypeId equals c.Id
                              join d in db.LoadRoute on a.OriginId equals d.Id
                              join e in db.LoadRoute on a.DestinationId equals e.Id
                              where b.ContractId.Equals(contractId)
                              select new ShippingFeeRouteWithPriceVM
                              {
                                  Id = a.Id,
                                  DestinationId = a.DestinationId,
                                  OriginId = a.OriginId,
                                  Origin = d.Title,
                                  Destination = e.Title,
                                  DriverPrice = b.DriverPrice,
                                  Price = b.Price,
                                  DriverTonnagePrice = b.DriverTonnagePrice,
                                  Vehicle = b.Vehicle,
                                  Title = b.Title,
                                  TonnagePrice = b.TonnagePrice,
                                  ShippingFeeLoadType = c.Name
                              }).AsNoTracking().ToListAsync();
            return data;
        }

        public void Update(ShippingFeeRoute obj)
        {
            db.Update(obj);
        }

        public void UpdateRange(List<ShippingFeeRoute> list)
        {
            db.UpdateRange(list.AsEnumerable());
        }
    }
}
