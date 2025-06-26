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
    public class FeedbackOrderRepository : IFeedbackOrderRepository
    {
        private readonly DbPhoneStoreContext _context;

        public FeedbackOrderRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<FeedbackOrder>> GetAllAsync()
        {
            return await _context.FeedbackOrders
                .Include(f => f.User)
                .Include(f => f.Order)
                .ToListAsync();
        }

        public async Task<FeedbackOrder?> GetByIdAsync(int id)
        {
            return await _context.FeedbackOrders
                .Include(f => f.User)
                .Include(f => f.Order)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(FeedbackOrder feedbackOrder)
        {
            feedbackOrder.CreatedAt ??= DateTime.UtcNow;

            await _context.FeedbackOrders.AddAsync(feedbackOrder);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FeedbackOrder feedbackOrder)
        {
            var existing = await _context.FeedbackOrders.FindAsync(feedbackOrder.Id);
            if (existing != null)
            {
                existing.UserId = feedbackOrder.UserId;
                existing.OrderId = feedbackOrder.OrderId;
                existing.Rating = feedbackOrder.Rating;
                existing.Comment = feedbackOrder.Comment;
                //existing.CreatedAt = feedbackOrder.CreatedAt;

                _context.FeedbackOrders.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.FeedbackOrders.FindAsync(id);
            if (existing != null)
            {
                _context.FeedbackOrders.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
