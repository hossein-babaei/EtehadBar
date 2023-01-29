using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<List<PaymentVM>> PaymentVMList(long calendarId, byte? type, long? vehicleId);
        Task<Payment> Get(long id);
        IQueryable<Payment> Payments();
        void Update(Payment obj);
        void Create(Payment obj);
        void Delete(Payment obj);
        Task<int> Save();
    }
}
