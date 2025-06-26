using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IDepositHistoryService
    {
        Task<List<DepositHistory>> GetAllAsync();
        Task<DepositHistory> GetByIdAsync(int id);
        Task<List<DepositHistory>> SearchAsync(string keyword); // Tìm theo PaymentMethod
        Task AddAsync(DepositHistory depositHistory);
        Task UpdateAsync(DepositHistory depositHistory);
        Task DeleteAsync(int id);
    }
}

