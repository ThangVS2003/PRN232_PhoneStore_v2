using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly DbPhoneStoreContext _context;

        public VoucherRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers
                .Include(v => v.Products)
                .Include(v => v.Orders)
                .ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(int id)
        {
            return await _context.Vouchers
                .Include(v => v.Products)
                .Include(v => v.Orders)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Voucher>> SearchAsync(string keyword)
        {
            var query = _context.Vouchers
                .Include(v => v.Products)
                .Include(v => v.Orders)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim(); // <-- loại bỏ khoảng trắng đầu/cuối
                query = query.Where(v => v.Code.Contains(keyword));
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Voucher voucher)
        {
            await _context.Vouchers.AddAsync(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Voucher voucher)
        {
            var existingVoucher = await _context.Vouchers.FindAsync(voucher.Id);
            if (existingVoucher != null)
            {
                existingVoucher.Code = voucher.Code;
                existingVoucher.DiscountValue = voucher.DiscountValue;
                existingVoucher.DiscountType = voucher.DiscountType;
                existingVoucher.MinOrderValue = voucher.MinOrderValue;

                //existingVoucher.ExpiryDate = voucher.ExpiryDate;
                //existingVoucher.IsActive = voucher.IsActive;
                existingVoucher.ApplyType = voucher.ApplyType;
                existingVoucher.Description = voucher.Description;

                _context.Vouchers.Update(existingVoucher);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ToggleActiveStatusAsync(int id)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Id == id);
            if (voucher == null) return false;

            voucher.IsActive = !(voucher.IsActive ?? false); // Đảo ngược trạng thái
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
