using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IVehicleBalanceRepository
    {
        Task<VehicleBalance> Get(long id);
        Task<VehicleBalance> Get(string rowId);
        Task<double> GetVehicleBalanceSum(long vehicleId, long? calendarId, long? customerId);
        Task<List<VehicleBalance>> GetVehicleBalance(long vehicleId, long? calendarId);
        IQueryable<VehicleBalance> Query();
        void Update(VehicleBalance obj);
        Task Create(VehicleBalance obj);
        void Delete(VehicleBalance obj);
        Task<int> Save();
    }
}
