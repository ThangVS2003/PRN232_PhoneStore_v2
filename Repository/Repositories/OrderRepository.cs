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
    public class OrderRepository : IOrderRepository
    {
        private readonly DbPhoneStoreContext _context;

        public OrderRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Voucher)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(pv => pv.Color)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(pv => pv.Version)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
               .Include(o => o.User)
               .Include(o => o.Voucher)
               .Include(o => o.OrderDetails)
               .ThenInclude(od => od.ProductVariant)
               .ThenInclude(pv => pv.Product)
               .Include(o => o.OrderDetails)
               .ThenInclude(od => od.ProductVariant)
               .ThenInclude(pv => pv.Color)
               .Include(o => o.OrderDetails)
               .ThenInclude(od => od.ProductVariant)
               .ThenInclude(pv => pv.Version)
               .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<Order> GetByUserIdAndStatusAsync(int userId, string status)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Voucher)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(pv => pv.Color)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .ThenInclude(pv => pv.Version)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == status);

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    Status = status,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0,
                    OrderDetails = new List<OrderDetail>()
                };
                await AddAsync(order);
            }

            return order;
        }
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(order.Id);
            if (existingOrder != null)
            {
                existingOrder.UserId = order.UserId;
                // existingOrder.OrderDate = order.OrderDate;
                existingOrder.TotalAmount = order.TotalAmount;
                existingOrder.Status = order.Status;
                existingOrder.ShippingAddress = order.ShippingAddress;
                existingOrder.VoucherId = order.VoucherId;

                _context.Orders.Update(existingOrder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}