using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IFeedbackOrderRepository
    {
        Task<List<FeedbackOrder>> GetAllAsync();
        Task<FeedbackOrder?> GetByIdAsync(int id);
        Task AddAsync(FeedbackOrder feedbackOrder);
        Task UpdateAsync(FeedbackOrder feedbackOrder);
        Task DeleteAsync(int id);
    }
}
