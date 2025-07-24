using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface ICartService
    {
        Task<Order> GetCartByUserIdAsync(int userId);
        Task AddToCartAsync(int userId, int productVariantId, int quantity);
        Task UpdateCartItemAsync(int orderDetailId, int quantity);
        Task RemoveCartItemAsync(int orderDetailId);
        Task ApplyVoucherAsync(int orderId, string voucherCode);
        Task<decimal> CalculateTotalAsync(int orderId);
        Task UpdateCartAsync(Order cart);
    }
}
