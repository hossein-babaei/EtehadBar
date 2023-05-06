using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<ActivityListVM>> ActivityList(long customerId, long calendarId, bool hasPayment);
        Task<Vehicle> Get(long id);
        IQueryable<Vehicle> Vehicles();
        void Update(Vehicle obj);
        void Create(Vehicle obj);
        Task<int> Save();
    }
}
