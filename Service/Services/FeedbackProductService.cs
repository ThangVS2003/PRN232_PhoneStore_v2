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
    public class FeedbackProductService : IFeedbackProductService
    {
        private readonly IFeedbackProductRepository _feedbackProductRepository;

        public FeedbackProductService(IFeedbackProductRepository feedbackProductRepository)
        {
            _feedbackProductRepository = feedbackProductRepository;
        }

        public async Task<List<FeedbackProduct>> GetAllAsync()
        {
            return await _feedbackProductRepository.GetAllAsync();
        }

        public async Task<FeedbackProduct> GetByIdAsync(int id)
        {
            return await _feedbackProductRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(FeedbackProduct feedbackProduct)
        {
            await _feedbackProductRepository.AddAsync(feedbackProduct);
        }

        public async Task UpdateAsync(FeedbackProduct feedbackProduct)
        {
            await _feedbackProductRepository.UpdateAsync(feedbackProduct);
        }

        public async Task DeleteAsync(int id)
        {
            await _feedbackProductRepository.DeleteAsync(id);
        }

        public async Task<List<FeedbackProduct>> GetByProductIdAsync(int productId)
        {
            return await _feedbackProductRepository.GetByProductIdAsync(productId);
        }
    }
}
