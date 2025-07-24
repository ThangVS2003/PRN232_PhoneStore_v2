using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IFeedbackOrderService
    {
        Task<List<FeedbackOrder>> GetAllAsync();
        Task<FeedbackOrder> GetByIdAsync(int id);
        Task AddAsync(FeedbackOrder feedbackOrder);
        Task UpdateAsync(FeedbackOrder feedbackOrder);
        Task DeleteAsync(int id);
        Task<List<FeedbackOrder>> GetByOrderIdAsync(int orderId);
    }
}
