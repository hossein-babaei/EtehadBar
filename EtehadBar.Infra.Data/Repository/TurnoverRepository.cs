using EtehadBar.Domain;
using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using MD.PersianDateTime.Standard;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class TurnoverRepository : ITurnoverRepository
    {
        private readonly ApplicationDbContext db;

        public TurnoverRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Turnover obj)
        {
            db.Add(obj);
        }

        public void Delete(Turnover obj)
        {
            db.Remove(obj);
        }

        public async Task<Turnover> Get(long id)
        {
            return await db.Turnover.FindAsync(id);
        }

        public async Task<Turnover> Get(string rowId)
        {
            return await db.Turnover.FirstOrDefaultAsync(a => a.RowId.Equals(rowId));
        }

        public async Task<EditTurnoverVM> GetEditData(long id)
        {
            var item = await db.Turnover.Where(a => a.Id.Equals(id)).AsNoTracking().SingleOrDefaultAsync(a => a.Id.Equals(id));
            var pd = new PersianDateTime(item.Date);
            return new EditTurnoverVM
            {
                Id = item.Id,
                Attachments = item.Attachments,
                Creditor = item.Creditor,
                Debtor = item.Debtor,
                Description = item.Description,
                TurnoverType = item.TurnoverType,
                UserId = item.UserId,
                Day = pd.Day,
                Month = pd.Month,
                Year = pd.Year,
            };
        }

        public IQueryable<Turnover> Query()
        {
            return db.Turnover.AsQueryable();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Turnover obj)
        {
            db.Update(obj);
        }
    }
}
