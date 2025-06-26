using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class DepositHistoryRepository : IDepositHistoryRepository
    {
        private readonly DbPhoneStoreContext _context;

        public DepositHistoryRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<DepositHistory>> GetAllAsync()
        {
            return await _context.DepositHistories
                .Include(d => d.User)
                .ToListAsync();
        }

        public async Task<DepositHistory?> GetByIdAsync(int id)
        {
            return await _context.DepositHistories
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<DepositHistory>> SearchByPaymentMethodAsync(string keyword)
        {
            var query = _context.DepositHistories
                .Include(d => d.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(d => d.PaymentMethod != null && d.PaymentMethod.Contains(keyword));
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(DepositHistory depositHistory)
        {
            depositHistory.DepositDate = depositHistory.DepositDate ?? DateTime.UtcNow;
            await _context.DepositHistories.AddAsync(depositHistory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DepositHistory depositHistory)
        {
            var existing = await _context.DepositHistories.FindAsync(depositHistory.Id);
            if (existing != null)
            {
                existing.UserId = depositHistory.UserId;
                existing.Amount = depositHistory.Amount;
                existing.DepositDate = depositHistory.DepositDate;
                existing.PaymentMethod = depositHistory.PaymentMethod;
                existing.Status = depositHistory.Status;

                _context.DepositHistories.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var deposit = await _context.DepositHistories.FindAsync(id);
            if (deposit != null)
            {
                _context.DepositHistories.Remove(deposit);
                await _context.SaveChangesAsync();
            }
        }
    }
}
