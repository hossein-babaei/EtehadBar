using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly ApplicationDbContext db;

        public CalendarRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Calendar obj)
        {
            db.Add(obj);
        }

        public IQueryable<Calendar> Calendars()
        {
            return db.Calendar.AsQueryable();
        }

        public void Delete(Calendar obj)
        {
            db.Remove(obj);
        }

        public async Task<Calendar> Get(long id)
        {
            return await db.Calendar.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Calendar obj)
        {
            db.Update(obj);
        }
    }
}
