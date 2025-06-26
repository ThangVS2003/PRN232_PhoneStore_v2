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
    public class DepositHistoryService : IDepositHistoryService
    {
        private readonly IDepositHistoryRepository _depositHistoryRepository;

        public DepositHistoryService(IDepositHistoryRepository depositHistoryRepository)
        {
            _depositHistoryRepository = depositHistoryRepository;
        }

        public async Task<List<DepositHistory>> GetAllAsync()
        {
            return await _depositHistoryRepository.GetAllAsync();
        }

        public async Task<DepositHistory> GetByIdAsync(int id)
        {
            return await _depositHistoryRepository.GetByIdAsync(id);
        }

        public async Task<List<DepositHistory>> SearchAsync(string keyword)
        {
            return await _depositHistoryRepository.SearchByPaymentMethodAsync(keyword);
        }

        public async Task AddAsync(DepositHistory depositHistory)
        {
            await _depositHistoryRepository.AddAsync(depositHistory);
        }

        public async Task UpdateAsync(DepositHistory depositHistory)
        {
            await _depositHistoryRepository.UpdateAsync(depositHistory);
        }

        public async Task DeleteAsync(int id)
        {
            await _depositHistoryRepository.DeleteAsync(id);
        }
    }
}