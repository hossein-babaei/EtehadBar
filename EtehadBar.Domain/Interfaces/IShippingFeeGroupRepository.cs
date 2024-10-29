using EtehadBar.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IShippingFeeGroupRepository
    {
        Task<ShippingFeeGroup> Get(long id);
        IQueryable<ShippingFeeGroup> Query();
        void Update(ShippingFeeGroup obj);
        void UpdateRange(List<ShippingFeeGroup> list);
        void UpdateLoadFactors(List<LoadFactor> list);
        void Create(ShippingFeeGroup obj);
        void Delete(ShippingFeeGroup obj);
        Task<int> Save();
    }
}
