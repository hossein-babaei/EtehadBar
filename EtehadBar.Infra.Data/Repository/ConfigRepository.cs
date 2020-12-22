using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly ApplicationDbContext db;

        public ConfigRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<string> CurrentYear()
        {
            return await db.Config.AsNoTracking().Select(a => a.Year).FirstAsync();
        }

        public async Task<Config> First()
        {
            return await db.Config.AsNoTracking().FirstAsync();
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Config obj)
        {
            db.Update(obj);
        }
    }
}
