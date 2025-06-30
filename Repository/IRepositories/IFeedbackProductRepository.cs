using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IFeedbackProductRepository
    {
        Task<List<FeedbackProduct>> GetAllAsync();
        Task<FeedbackProduct?> GetByIdAsync(int id);
        Task AddAsync(FeedbackProduct feedbackProduct);
        Task UpdateAsync(FeedbackProduct feedbackProduct);
        Task DeleteAsync(int id);
        Task<List<FeedbackProduct>> GetByProductIdAsync(int productId);
    }
}

