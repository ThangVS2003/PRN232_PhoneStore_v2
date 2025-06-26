using BusinessObject.Models;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class FeedbackOrderService : IFeedbackOrderService
    {
        private readonly IFeedbackOrderRepository _feedbackOrderRepository;

        public FeedbackOrderService(IFeedbackOrderRepository feedbackOrderRepository)
        {
            _feedbackOrderRepository = feedbackOrderRepository;
        }

        public async Task<List<FeedbackOrder>> GetAllAsync()
        {
            return await _feedbackOrderRepository.GetAllAsync();
        }

        public async Task<FeedbackOrder> GetByIdAsync(int id)
        {
            return await _feedbackOrderRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(FeedbackOrder feedbackOrder)
        {
            await _feedbackOrderRepository.AddAsync(feedbackOrder);
        }

        public async Task UpdateAsync(FeedbackOrder feedbackOrder)
        {
            await _feedbackOrderRepository.UpdateAsync(feedbackOrder);
        }

        public async Task DeleteAsync(int id)
        {
            await _feedbackOrderRepository.DeleteAsync(id);
        }
    }
}
