using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> Get(int id);
        IQueryable<Payment> Payments();
        void Update(Payment obj);
        void Create(Payment obj);
        void Delete(Payment obj);
        Task<int> Save();
    }
}
