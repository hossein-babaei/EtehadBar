using EtehadBar.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ICalendarRepository
    {
        Task<Calendar> Get(string id);
        IQueryable<Calendar> Calendars();
        void Update(Calendar obj);
        void Create(Calendar obj);
        void Delete(Calendar obj);
        Task<int> Save();
    }
}
