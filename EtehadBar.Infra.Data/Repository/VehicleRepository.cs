using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext db;

        public VehicleRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Vehicle obj)
        {
            db.Add(obj);
        }

        public IQueryable<Vehicle> Vehicles()
        {
            return db.Vehicles.AsQueryable();
        }

        public async Task<Vehicle> GetVehicle(string id)
        {
            return await db.Vehicles.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Vehicle obj)
        {
            db.Update(obj);
        }
    }
}
