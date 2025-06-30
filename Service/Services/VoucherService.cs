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
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<List<Voucher>> GetAllAsync()
        {
            return await _voucherRepository.GetAllAsync();
        }

        public async Task<Voucher> GetByIdAsync(int id)
        {
            return await _voucherRepository.GetByIdAsync(id);
        }
        public async Task<Voucher?> GetVoucherByCodeAsync(string code)
        {
            return await _voucherRepository.GetByCodeAsync(code);
        }
        public async Task<List<Voucher>> SearchAsync(string keyword)
        {
            return await _voucherRepository.SearchAsync(keyword);
        }

        public async Task AddAsync(Voucher voucher)
        {
            await _voucherRepository.AddAsync(voucher);
        }

        public async Task UpdateAsync(Voucher voucher)
        {
            await _voucherRepository.UpdateAsync(voucher);
        }

        public async Task DeleteAsync(int id)
        {
            await _voucherRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleActiveStatusAsync(int id)
        {
            return await _voucherRepository.ToggleActiveStatusAsync(id);
        }
    }
}
