using EtehadBar.Domain.Interfaces;
using EtehadBar.Domain.Models;
using EtehadBar.Infra.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace EtehadBar.Infra.Data.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext db;

        public PaymentRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public void Create(Payment obj)
        {
            db.Add(obj);
        }

        public IQueryable<Payment> Payments()
        {
            return db.Payment.AsQueryable();
        }

        public void Delete(Payment obj)
        {
            db.Remove(obj);
        }

        public async Task<Payment> Get(int id)
        {
            return await db.Payment.FindAsync(id);
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Update(Payment obj)
        {
            db.Update(obj);
        }
    }
}
