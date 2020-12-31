using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface ILoadFactorRepository
    {
        Task<LoadFactor> Get(string id);
        Task<ESaipaPlascoLoadFactorVM> GetSaipaPlascoLoadFactor(string loadFactorId);
        Task<ESaipaPressLoadFactorVM> GetSaipaPressLoadFactor(string loadFactorId);
        Task<ESazehGostarLoadFactorVM> GetSazehGostarLoadFactor(string loadFactorId);
        IQueryable<LoadFactor> LoadFactors();
        void Update(LoadFactor obj);
        void UpdateSaipaPress(SaipaPressLoadFactor obj);
        void UpdateSazehGostar(SazehGostarLoadFactor obj);
        void UpdateRange(List<LoadFactor> list);
        void Create(LoadFactor obj);
        void CreateSaipaPress(SaipaPressLoadFactor obj);
        void CreateSazehGostar(SazehGostarLoadFactor obj);
        void Delete(LoadFactor obj);
        Task<int> Save();
    }
}
