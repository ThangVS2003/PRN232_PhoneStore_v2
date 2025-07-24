using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using System.Threading.Tasks;
using System.Security.Claims;
using Repository.IRepositories;
using Microsoft.Extensions.Logging;
using Repository.IRepository;
using Service.IService;
using Microsoft.AspNetCore.Authorization;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IVoucherRepository _voucherRepository;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            IUserService userService,
            IVoucherRepository voucherRepository,
            ILogger<CartController> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _voucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                if (dto == null || dto.ProductVariantId <= 0 || dto.Quantity <= 0)
                {
                    _logger.LogWarning("Dữ liệu AddToCartDto không hợp lệ: {Dto}", dto);
                    return BadRequest(new { success = false, message = "Dữ liệu đầu vào không hợp lệ." });
                }

                var userId = GetCurrentUserId();
                await _cartService.AddToCartAsync(userId, dto.ProductVariantId, dto.Quantity);
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                _logger.LogInformation("Đã thêm sản phẩm vào giỏ hàng cho user {UserId}. Số lượng mục: {CartCount}", userId, cart.OrderDetails.Count);
                return Ok(new { success = true, cartCount = cart.OrderDetails.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm vào giỏ hàng cho user. Request: {Dto}", dto);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy giỏ hàng cho user {UserId}", userId);
                    return Ok(new
                    {
                        OrderDetails = new List<object>(),
                        Total = 0,
                        ShippingAddress = "Chưa có địa chỉ",
                        VoucherCode = "N/A"
                    });
                }

                var user = await _userService.GetByIdAsync(userId);
                var result = new
                {
                    OrderDetails = cart.OrderDetails.Select(od => new
                    {
                        od.Id,
                        ProductVariant = new
                        {
                            od.ProductVariant.Id,
                            Name = od.ProductVariant.Product?.Name ?? "Sản phẩm không xác định",
                            Color = od.ProductVariant.Color != null ? od.ProductVariant.Color.Name : "N/A",
                            Version = od.ProductVariant.Version != null ? od.ProductVariant.Version.Name : "N/A",
                            od.ProductVariant.SellingPrice,
                            od.ProductVariant.Image
                        },
                        od.Quantity
                    }).ToList(),
                    Total = await _cartService.CalculateTotalAsync(cart.Id),
                    ShippingAddress = user?.Address ?? "Chưa có địa chỉ",
                    VoucherCode = cart.VoucherId.HasValue ? (await _voucherRepository.GetByIdAsync(cart.VoucherId.Value))?.Code ?? "N/A" : "N/A"
                };

                _logger.LogInformation("Lấy giỏ hàng thành công cho user {UserId}. Số lượng mục: {CartCount}", userId, cart.OrderDetails.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy giỏ hàng cho user");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("apply-voucher")]
        [Authorize]
        public async Task<IActionResult> ApplyVoucher([FromBody] ApplyVoucherDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto?.VoucherCode))
                {
                    _logger.LogWarning("Mã voucher không hợp lệ: {VoucherCode}", dto?.VoucherCode);
                    return BadRequest(new { success = false, message = "Mã voucher không hợp lệ." });
                }

                var userId = GetCurrentUserId();
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy giỏ hàng cho user {UserId}", userId);
                    return BadRequest(new { success = false, message = "Không tìm thấy giỏ hàng." });
                }

                await _cartService.ApplyVoucherAsync(cart.Id, dto.VoucherCode);
                var total = await _cartService.CalculateTotalAsync(cart.Id);
                _logger.LogInformation("Áp dụng voucher {VoucherCode} thành công cho user {UserId}. Tổng mới: {Total}", dto.VoucherCode, userId, total);
                return Ok(new { success = true, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi áp dụng voucher {VoucherCode} cho user", dto?.VoucherCode);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update/{orderDetailId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItem(int orderDetailId, [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                if (dto == null || dto.Quantity <= 0)
                {
                    _logger.LogWarning("Dữ liệu UpdateCartItemDto không hợp lệ: {Dto}", dto);
                    return BadRequest(new { success = false, message = "Số lượng không hợp lệ." });
                }

                await _cartService.UpdateCartItemAsync(orderDetailId, dto.Quantity);
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                var total = await _cartService.CalculateTotalAsync(cart.Id);
                _logger.LogInformation("Cập nhật mục giỏ hàng {OrderDetailId} cho user {UserId}. Tổng mới: {Total}", orderDetailId, userId, total);
                return Ok(new { success = true, cartCount = cart.OrderDetails.Count, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật mục giỏ hàng {OrderDetailId} cho user", orderDetailId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("remove/{orderDetailId}")]
        [Authorize]
        public async Task<IActionResult> RemoveCartItem(int orderDetailId)
        {
            try
            {
                await _cartService.RemoveCartItemAsync(orderDetailId);
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                var total = await _cartService.CalculateTotalAsync(cart.Id);
                _logger.LogInformation("Xóa mục giỏ hàng {OrderDetailId} cho user {UserId}. Tổng mới: {Total}", orderDetailId, userId, total);
                return Ok(new { success = true, cartCount = cart.OrderDetails.Count, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa mục giỏ hàng {OrderDetailId} cho user", orderDetailId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("remove-voucher")]
        [Authorize]
        public async Task<IActionResult> RemoveVoucher()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation("Nhận yêu cầu xóa voucher cho user {UserId}", userId);
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy giỏ hàng cho user {UserId}", userId);
                    return BadRequest(new { success = false, message = "Không tìm thấy giỏ hàng." });
                }

                if (!cart.VoucherId.HasValue)
                {
                    _logger.LogWarning("Giỏ hàng của user {UserId} không có voucher để xóa", userId);
                    return Ok(new { success = true, total = await _cartService.CalculateTotalAsync(cart.Id) });
                }

                cart.VoucherId = null;
                await _cartService.UpdateCartAsync(cart);
                var total = await _cartService.CalculateTotalAsync(cart.Id);
                _logger.LogInformation("Xóa voucher thành công cho user {UserId}. Tổng mới: {Total}", userId, total);
                return Ok(new { success = true, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa voucher cho user");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("count")]
        [Authorize]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                _logger.LogInformation("Lấy số lượng mục giỏ hàng cho user {UserId}: {CartCount}", userId, cart.OrderDetails.Count);
                return Ok(new { cartCount = cart.OrderDetails.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy số lượng mục giỏ hàng cho user");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new Exception("Người dùng chưa đăng nhập.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Không tìm thấy claim ID người dùng trong token.");
                throw new Exception("Không tìm thấy ID người dùng trong token.");
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Claim ID người dùng không hợp lệ: {UserIdClaim}", userIdClaim);
                throw new Exception("ID người dùng không hợp lệ.");
            }

            _logger.LogInformation("Lấy được ID người dùng: {UserId}", userId);
            return userId;
        }
    }

    public class AddToCartDto
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class ApplyVoucherDto
    {
        public string VoucherCode { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }
}