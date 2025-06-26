using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IDepositHistoryRepository
    {
        Task<List<DepositHistory>> GetAllAsync();
        Task<DepositHistory?> GetByIdAsync(int id);
        Task<List<DepositHistory>> SearchByPaymentMethodAsync(string keyword);
        Task AddAsync(DepositHistory depositHistory);
        Task UpdateAsync(DepositHistory depositHistory);
        Task DeleteAsync(int id);
    }
}

