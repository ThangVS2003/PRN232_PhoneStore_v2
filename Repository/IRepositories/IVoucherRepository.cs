using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IVoucherRepository
    {
        Task<List<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(int id);
        Task<Voucher?> GetByCodeAsync(string code);
        Task<List<Voucher>> SearchAsync(string keyword);
        Task AddAsync(Voucher voucher);
        Task UpdateAsync(Voucher voucher);
        Task DeleteAsync(int id);
    }
}
