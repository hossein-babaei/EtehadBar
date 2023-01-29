using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ILoadFactorRepository
    {
        Task<LoadFactor> Get(long id);
        Task<ESaipaPlascoLoadFactorVM> GetSaipaPlascoLoadFactor(long loadFactorId);
        Task<ESaipaPressLoadFactorVM> GetSaipaPressLoadFactor(long loadFactorId);
        Task<ESazehGostarLoadFactorVM> GetSazehGostarLoadFactor(long loadFactorId);
        IQueryable<LoadFactor> LoadFactors();
        Task<List<LoadFactor>> LoadFactors(long customerId, long calendarId);
        void Update(LoadFactor obj);
        void UpdateSaipaPress(SaipaPressLoadFactor obj);
        void UpdateSazehGostar(SazehGostarLoadFactor obj);
        void UpdateRange(List<LoadFactor> list);
        void Create(LoadFactor obj);
        void CreateSaipaPress(SaipaPressLoadFactor obj);
        void CreateSazehGostar(SazehGostarLoadFactor obj);
        void Delete(LoadFactor obj);
        void DeleteSaipaPress(SaipaPressLoadFactor obj);
        void DeleteSazehGostar(SazehGostarLoadFactor obj);
        Task<int> Save();
    }
}
