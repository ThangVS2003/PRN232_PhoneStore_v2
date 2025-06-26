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
    public class FeedbackProductRepository : IFeedbackProductRepository
    {
        private readonly DbPhoneStoreContext _context;

        public FeedbackProductRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<FeedbackProduct>> GetAllAsync()
        {
            return await _context.FeedbackProducts
                                 .Include(fp => fp.Product)
                                 .Include(fp => fp.User)
                                 .ToListAsync();
        }

        public async Task<FeedbackProduct?> GetByIdAsync(int id)
        {
            return await _context.FeedbackProducts
                                 .Include(fp => fp.Product)
                                 .Include(fp => fp.User)
                                 .FirstOrDefaultAsync(fp => fp.Id == id);
        }

        public async Task AddAsync(FeedbackProduct feedbackProduct)
        {
            feedbackProduct.CreatedAt = DateTime.UtcNow;
            _context.FeedbackProducts.Add(feedbackProduct);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FeedbackProduct feedbackProduct)
        {
            var existing = await _context.FeedbackProducts.FirstOrDefaultAsync(fp => fp.Id == feedbackProduct.Id);
            if (existing != null)
            {
                existing.Rating = feedbackProduct.Rating;
                existing.Comment = feedbackProduct.Comment;
                existing.UserId = feedbackProduct.UserId;
                existing.ProductId = feedbackProduct.ProductId;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var feedback = await _context.FeedbackProducts.FirstOrDefaultAsync(fp => fp.Id == id);
            if (feedback != null)
            {
                _context.FeedbackProducts.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }
    }
}
