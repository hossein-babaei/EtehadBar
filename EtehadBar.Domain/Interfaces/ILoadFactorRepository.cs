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
        Task<EMehrcomParsLoadFactorVM> GetMehrcomParsLoadFactor(long loadFactorId);
        Task<long> GetBiggestSequenceInSaipaPlasco();
        Task<long> GetBiggestSequenceInSaipaPress();
        Task<long> GetBiggestSequenceInSazehGostar();
        Task<long> GetBiggestSequenceInMehrcomPars();
        Task<bool> SequenceExistInSaipaPlasco(long id, long sequence);
        Task<bool> SequenceExistInSaipaPress(long id, long sequence);
        Task<bool> SequenceExistInSazehGostar(long id, long sequence);
        Task<bool> SequenceExistInMehrcomPars(long id, long sequence);
        Task<bool> CheckMehrcomParsLoadFactorGovNumber(string number);
        IQueryable<LoadFactor> LoadFactors();
        Task<List<ExcelLoadFactorVM>> LoadFactors(long customerId, long? calendarId, long? accountBookId, long? driverId);
        void Update(LoadFactor obj);
        void UpdateSaipaPress(SaipaPressLoadFactor obj);
        void UpdateSazehGostar(SazehGostarLoadFactor obj);
        void UpdateSaipaPlasco(SaipaPlascoLoadFactor obj);
        void UpdateMehrcomPars(MehrcomParsLoadFactor obj);
        void UpdateRange(List<LoadFactor> list);
        void Create(LoadFactor obj);
        void CreateSaipaPress(SaipaPressLoadFactor obj);
        void CreateSazehGostar(SazehGostarLoadFactor obj);
        void CreateSaipaPlasco(SaipaPlascoLoadFactor obj);
        void CreateMehrcomPars(MehrcomParsLoadFactor obj);
        void Delete(LoadFactor obj);
        void DeleteSaipaPress(SaipaPressLoadFactor obj);
        void DeleteSazehGostar(SazehGostarLoadFactor obj);
        void DeleteMehrcomPars(MehrcomParsLoadFactor obj);
        Task<int> Save();
    }
}
