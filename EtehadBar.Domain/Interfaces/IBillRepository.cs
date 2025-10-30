using EtehadBar.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Domain.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> Get(long id);
        Task<Bill> Get(string rowId);
        Task<Bill> GetIncludedDetail(long id);
        Task<EditBillVM> GetEditData(long id);
        IQueryable<Bill> Query();
        void Update(Bill obj);
        void UpdateRange(List<Bill> bills);
        void Create(Bill obj);
        void Delete(Bill obj);
        void DeleteBillDetail(BillDetail obj);
        Task<int> Save();
    }
}
