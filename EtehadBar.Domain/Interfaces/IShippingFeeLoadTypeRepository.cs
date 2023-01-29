using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IShippingFeeLoadTypeRepository
    {
        Task<ShippingFeeLoadType> Get(long id);
        IQueryable<ShippingFeeLoadType> ShippingFeeLoadTypes();
        void Update(ShippingFeeLoadType obj);
        void UpdateRange(List<ShippingFeeLoadType> list);
        void Create(ShippingFeeLoadType obj);
        void Delete(ShippingFeeLoadType obj);
        Task<int> Save();
    }
}
