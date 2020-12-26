using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IShippingFeeRepository
    {
        Task<ShippingFee> Get(string id);
        IQueryable<ShippingFee> ShippingFees();
        void Update(ShippingFee obj);
        void Create(ShippingFee obj);
        void Delete(ShippingFee obj);
        Task<int> Save();
    }
}
