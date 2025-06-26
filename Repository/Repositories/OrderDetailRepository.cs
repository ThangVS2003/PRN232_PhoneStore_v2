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
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly DbPhoneStoreContext _context;

        public OrderDetailRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.ProductVariant)
                .ToListAsync();
        }

        public async Task<OrderDetail?> GetByIdAsync(int id)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.ProductVariant)
                .FirstOrDefaultAsync(od => od.Id == id);
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderDetail orderDetail)
        {
            var existing = await _context.OrderDetails.FindAsync(orderDetail.Id);
            if (existing != null)
            {
                existing.OrderId = orderDetail.OrderId;
                existing.ProductVariantId = orderDetail.ProductVariantId;
                existing.Quantity = orderDetail.Quantity;
                existing.UnitPrice = orderDetail.UnitPrice;

                _context.OrderDetails.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}