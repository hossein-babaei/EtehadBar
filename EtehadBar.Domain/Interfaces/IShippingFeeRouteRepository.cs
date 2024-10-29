using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IShippingFeeRouteRepository
    {
        Task<ShippingFeeRoute> Get(long id);
        Task<ShippingFeeRoute> GetWithGroup(long id);
        Task<ShippingFeeRoute> GetWithGroupAndLoadRoute(long id);
        Task<List<ShippingFeeRouteWithPriceVM>> ShippingFeeRouteWithPrice(long contractId);
        IQueryable<ShippingFeeRoute> Query();
        void Update(ShippingFeeRoute obj);
        void UpdateRange(List<ShippingFeeRoute> list);
        void Create(ShippingFeeRoute obj);
        void Delete(ShippingFeeRoute obj);
        Task<int> Save();
    }
}
