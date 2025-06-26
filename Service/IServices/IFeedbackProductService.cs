using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IFeedbackProductService
    {
        Task<List<FeedbackProduct>> GetAllAsync();
        Task<FeedbackProduct> GetByIdAsync(int id);
        Task AddAsync(FeedbackProduct feedbackProduct);
        Task UpdateAsync(FeedbackProduct feedbackProduct);
        Task DeleteAsync(int id);
    }
}

