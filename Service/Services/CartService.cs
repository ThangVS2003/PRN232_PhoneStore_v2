using BusinessObject.Models;
using Repository.IRepository;
using Service.IService;
using Service.IServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CartService : ICartService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IVoucherRepository _voucherRepository;

        public CartService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IProductVariantRepository productVariantRepository,
            IVoucherRepository voucherRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productVariantRepository = productVariantRepository;
            _voucherRepository = voucherRepository;
        }

        public async Task<Order> GetCartByUserIdAsync(int userId)
        {
            var cart = await _orderRepository.GetByUserIdAndStatusAsync(userId, "Cart");
            if (cart == null)
            {
                cart = new Order
                {
                    UserId = userId,
                    Status = "Cart",
                    OrderDate = DateTime.Now,
                    TotalAmount = 0,
                    OrderDetails = new List<OrderDetail>()
                };
                await _orderRepository.AddAsync(cart);
            }
            return cart;
        }

        public async Task AddToCartAsync(int userId, int productVariantId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var productVariant = await _productVariantRepository.GetByIdAsync(productVariantId);
            if (productVariant == null || productVariant.IsDeleted == true || productVariant.StockQuantity < quantity)
            {
                throw new Exception("Sản phẩm không tồn tại hoặc không đủ số lượng.");
            }

            var orderDetail = cart.OrderDetails.FirstOrDefault(od => od.ProductVariantId == productVariantId);
            if (orderDetail != null)
            {
                orderDetail.Quantity += quantity;
                orderDetail.UnitPrice = productVariant.SellingPrice;
                await _orderDetailRepository.UpdateAsync(orderDetail);
            }
            else
            {
                orderDetail = new OrderDetail
                {
                    OrderId = cart.Id,
                    ProductVariantId = productVariantId,
                    Quantity = quantity,
                    UnitPrice = productVariant.SellingPrice
                };
                await _orderDetailRepository.AddAsync(orderDetail);
            }

            cart.TotalAmount = await CalculateTotalAsync(cart.Id);
            await _orderRepository.UpdateAsync(cart);
        }

        public async Task UpdateCartItemAsync(int orderDetailId, int quantity)
        {
            var orderDetail = await _orderDetailRepository.GetByIdAsync(orderDetailId);
            if (orderDetail == null)
            {
                throw new Exception("Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(orderDetail.ProductVariantId);
            if (productVariant == null || productVariant.IsDeleted == true || productVariant.StockQuantity < quantity)
            {
                throw new Exception("Sản phẩm không tồn tại hoặc không đủ số lượng.");
            }

            orderDetail.Quantity = quantity;
            orderDetail.UnitPrice = productVariant.SellingPrice;
            await _orderDetailRepository.UpdateAsync(orderDetail);

            var cart = await _orderRepository.GetByIdAsync(orderDetail.OrderId);
            cart.TotalAmount = await CalculateTotalAsync(cart.Id);
            await _orderRepository.UpdateAsync(cart);
        }

        public async Task RemoveCartItemAsync(int orderDetailId)
        {
            var orderDetail = await _orderDetailRepository.GetByIdAsync(orderDetailId);
            if (orderDetail == null)
            {
                throw new Exception("Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            await _orderDetailRepository.DeleteAsync(orderDetailId);

            var cart = await _orderRepository.GetByIdAsync(orderDetail.OrderId);
            cart.TotalAmount = await CalculateTotalAsync(cart.Id);
            await _orderRepository.UpdateAsync(cart);
        }

        public async Task ApplyVoucherAsync(int orderId, string voucherCode)
        {
            var cart = await _orderRepository.GetByIdAsync(orderId);
            if (cart == null || cart.Status != "Cart")
            {
                throw new Exception("Không tìm thấy giỏ hàng.");
            }

            var voucher = await _voucherRepository.GetByCodeAsync(voucherCode);
            if (voucher == null || voucher.IsActive != true || voucher.ExpiryDate < DateTime.Now)
            {
                throw new Exception("Voucher không hợp lệ hoặc đã hết hạn.");
            }

            var total = await CalculateTotalAsync(orderId);
            if (voucher.MinOrderValue.HasValue && total < voucher.MinOrderValue)
            {
                throw new Exception("Giá trị đơn hàng không đủ để áp dụng voucher.");
            }

            cart.VoucherId = voucher.Id;
            cart.TotalAmount = await CalculateTotalAsync(orderId);
            await _orderRepository.UpdateAsync(cart);
        }

        public async Task<decimal> CalculateTotalAsync(int orderId)
        {
            var cart = await _orderRepository.GetByIdAsync(orderId);
            if (cart == null)
            {
                return 0;
            }

            decimal total = 0;
            foreach (var item in cart.OrderDetails)
            {
                var variant = await _productVariantRepository.GetByIdAsync(item.ProductVariantId);
                if (variant != null && variant.IsDeleted == false)
                {
                    total += item.UnitPrice * item.Quantity;
                }
            }

            var voucher = cart.VoucherId.HasValue ? await _voucherRepository.GetByIdAsync(cart.VoucherId.Value) : null;
            if (voucher != null && voucher.IsActive == true && voucher.ExpiryDate >= DateTime.Now)
            {
                if (voucher.DiscountType == "Percentage")
                {
                    total -= total * (voucher.DiscountValue / 100);
                }
                else if (voucher.DiscountType == "Fixed")
                {
                    total -= voucher.DiscountValue;
                }
            }

            return total > 0 ? total : 0;
        }
    }
}