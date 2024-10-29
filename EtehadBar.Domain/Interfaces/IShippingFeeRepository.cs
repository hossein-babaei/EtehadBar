using EtehadBar.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IShippingFeeRepository
    {
        Task<ShippingFee> Get(long id);
        IQueryable<ShippingFee> ShippingFees();
        void Update(ShippingFee obj);
        void UpdateRange(List<ShippingFee> list);
        void UpdateLoadFactors(List<LoadFactor> list);
        void Create(ShippingFee obj);
        void Delete(ShippingFee obj);
        Task<int> Save();
    }
}
