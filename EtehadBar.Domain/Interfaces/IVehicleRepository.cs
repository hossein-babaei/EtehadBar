using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<ActivityListVM>> ActivityList(long customerId, long calendarId, bool hasPayment, bool isFreeDriverPrice);
        Task<List<ActivityListVM>> FullActivityList(long customerId, long calendarId, int type);
        Task<List<ActivityListByCustomerVM>> ActivityListByCustomer(long customerId, long calendarId, bool isFreeDriverPrice);
        Task<Vehicle> Get(long id);
        IQueryable<Vehicle> Vehicles();
        void Update(Vehicle obj);
        void Create(Vehicle obj);
        Task<int> Save();
    }
}
