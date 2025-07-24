using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartController> _logger;

        public CartController(IHttpClientFactory httpClientFactory, ILogger<CartController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                _logger.LogInformation("JwtToken from cookie: {Token}", token ?? "null");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy token JWT trong cookie. Chuyển hướng đến trang đăng nhập.");
                    return RedirectToAction("Login", "Account");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetFromJsonAsync<CartViewModel>("api/Cart");
                if (response == null)
                {
                    _logger.LogWarning("Không lấy được dữ liệu giỏ hàng từ API.");
                    return View(new CartViewModel
                    {
                        OrderDetails = new List<CartViewModel.OrderDetailViewModel>(),
                        ShippingAddress = "Chưa có địa chỉ",
                        VoucherCode = "N/A",
                        Total = 0
                    });
                }
                _logger.LogInformation("Lấy giỏ hàng thành công. Số lượng mục: {CartCount}", response.OrderDetails.Count);
                return View(response);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, "Lỗi xác thực khi gọi API giỏ hàng. Token có thể không hợp lệ.");
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải giỏ hàng");
                ModelState.AddModelError("", $"Lỗi khi tải giỏ hàng: {ex.Message}");
                return View(new CartViewModel
                {
                    OrderDetails = new List<CartViewModel.OrderDetailViewModel>(),
                    ShippingAddress = "Chưa có địa chỉ",
                    VoucherCode = "N/A",
                    Total = 0
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int variantId, int quantity)
        {
            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                _logger.LogInformation("JwtToken from cookie for AddToCart: {Token}", token ?? "null");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy token JWT khi thêm sản phẩm vào giỏ hàng.");
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.", redirectToLogin = true });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsJsonAsync("api/Cart/add", new { productVariantId = variantId, quantity });
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<AddToCartResponse>();
                _logger.LogInformation("Thêm sản phẩm vào giỏ hàng thành công. Số lượng mục: {CartCount}", result.cartCount);
                return Json(new { success = true, cartCount = result.cartCount });
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, "Lỗi xác thực khi gọi API thêm sản phẩm vào giỏ hàng.");
                return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.", redirectToLogin = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm vào giỏ hàng");
                return Json(new { success = false, message = $"Không thể thêm sản phẩm vào giỏ hàng: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("Cart/UpdateCartItem/{orderDetailId}")]
        public async Task<IActionResult> UpdateCartItem(int orderDetailId, [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                _logger.LogInformation("JwtToken from cookie for UpdateCartItem: {Token}", token ?? "null");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy token JWT khi cập nhật giỏ hàng.");
                    return Json(new { success = false, message = "Vui lòng đăng nhập để cập nhật giỏ hàng.", redirectToLogin = true });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PutAsJsonAsync($"api/Cart/update/{orderDetailId}", dto);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<UpdateCartResponse>();
                _logger.LogInformation("Cập nhật giỏ hàng thành công. Số lượng mục: {CartCount}, Tổng: {Total}", result.cartCount, result.total);
                return Json(new { success = true, cartCount = result.cartCount, total = result.total });
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, "Lỗi xác thực khi gọi API cập nhật giỏ hàng.");
                return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.", redirectToLogin = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật mục giỏ hàng {OrderDetailId}", orderDetailId);
                return Json(new { success = false, message = $"Không thể cập nhật số lượng: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("Cart/RemoveCartItem/{orderDetailId}")]
        public async Task<IActionResult> RemoveCartItem(int orderDetailId)
        {
            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                _logger.LogInformation("JwtToken from cookie for RemoveCartItem: {Token}", token ?? "null");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy token JWT khi xóa mục giỏ hàng.");
                    return Json(new { success = false, message = "Vui lòng đăng nhập để xóa sản phẩm.", redirectToLogin = true });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.DeleteAsync($"api/Cart/remove/{orderDetailId}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<RemoveCartResponse>();
                _logger.LogInformation("Xóa mục giỏ hàng {OrderDetailId} thành công. Số lượng mục: {CartCount}, Tổng: {Total}", orderDetailId, result.cartCount, result.total);
                return Json(new { success = true, cartCount = result.cartCount, total = result.total });
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, "Lỗi xác thực khi gọi API xóa mục giỏ hàng.");
                return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.", redirectToLogin = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa mục giỏ hàng {OrderDetailId}", orderDetailId);
                return Json(new { success = false, message = $"Không thể xóa sản phẩm: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApplyVoucher([FromBody] ApplyVoucherRequestDto request)
        {
            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                _logger.LogInformation("JwtToken from cookie for ApplyVoucher: {Token}", token ?? "null");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy token JWT khi áp dụng voucher.");
                    return Json(new { success = false, message = "Vui lòng đăng nhập để áp dụng voucher.", redirectToLogin = true });
                }

                if (string.IsNullOrEmpty(request?.voucherCode))
                {
                    _logger.LogWarning("Mã voucher không hợp lệ: {VoucherCode}", request?.voucherCode);
                    return Json(new { success = false, message = "Vui lòng nhập mã voucher." });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var requestBody = new { VoucherCode = request.voucherCode }; // Đảm bảo gửi VoucherCode với chữ V hoa
                var response = await _httpClient.PostAsJsonAsync("api/Cart/apply-voucher", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApplyVoucherResponse>();
                    _logger.LogInformation("Áp dụng voucher {VoucherCode} thành công. Tổng mới: {Total}", request.voucherCode, result.total);
                    return Json(new { success = true, total = result.total });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Lỗi khi áp dụng voucher {VoucherCode}. Mã trạng thái: {StatusCode}, Nội dung lỗi: {ErrorContent}", request.voucherCode, response.StatusCode, errorContent);
                    var errorMessage = string.IsNullOrEmpty(errorContent) ? "Không thể áp dụng voucher." : errorContent;
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, "Lỗi xác thực khi gọi API áp dụng voucher.");
                return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.", redirectToLogin = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi áp dụng voucher {VoucherCode}", request?.voucherCode);
                return Json(new { success = false, message = $"Không thể áp dụng voucher: {ex.Message}" });
            }
        }
        public class ApplyVoucherRequestDto
        {
            public string voucherCode { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveVoucher()
        {
            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                _logger.LogInformation("JwtToken from cookie for RemoveVoucher: {Token}", token ?? "null");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy token JWT khi xóa voucher.");
                    return Json(new { success = false, message = "Vui lòng đăng nhập để xóa voucher.", redirectToLogin = true });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync("api/Cart/remove-voucher", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RemoveVoucherResponse>();
                    _logger.LogInformation("Xóa voucher thành công. Tổng mới: {Total}", result.total);
                    return Json(new { success = true, total = result.total });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Lỗi khi xóa voucher. Mã trạng thái: {StatusCode}, Nội dung lỗi: {ErrorContent}", response.StatusCode, errorContent);
                    var errorMessage = string.IsNullOrEmpty(errorContent) ? "Không thể xóa voucher." : errorContent;
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, "Lỗi xác thực khi gọi API xóa voucher.");
                return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.", redirectToLogin = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa voucher");
                return Json(new { success = false, message = $"Không thể xóa voucher: {ex.Message}" });
            }
        }
        public class RemoveVoucherResponse
        {
            public bool success { get; set; }
            public decimal total { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                _logger.LogInformation("JwtToken from cookie for GetCartCount: {Token}", token ?? "null");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy token JWT khi lấy số lượng giỏ hàng.");
                    return Json(new { success = false, cartCount = 0, message = "Vui lòng đăng nhập để xem giỏ hàng.", redirectToLogin = true });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetFromJsonAsync<CartCountResponse>("api/Cart/count");
                if (response == null)
                {
                    _logger.LogWarning("Không lấy được số lượng giỏ hàng từ API.");
                    return Json(new { success = false, cartCount = 0, message = "Không thể lấy số lượng giỏ hàng." });
                }
                _logger.LogInformation("Lấy số lượng giỏ hàng thành công: {CartCount}", response.cartCount);
                return Json(new { success = true, cartCount = response.cartCount });
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, "Lỗi xác thực khi gọi API lấy số lượng giỏ hàng.");
                return Json(new { success = false, cartCount = 0, message = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.", redirectToLogin = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy số lượng giỏ hàng");
                return Json(new { success = false, cartCount = 0, message = $"Không thể lấy số lượng giỏ hàng: {ex.Message}" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("UserIdClaim: {UserIdClaim}", userIdClaim ?? "null");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Không tìm thấy claim ID người dùng trong token.");
                throw new Exception("Người dùng chưa đăng nhập.");
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

    public class CartViewModel
    {
        public List<OrderDetailViewModel> OrderDetails { get; set; }
        public string ShippingAddress { get; set; }
        public string VoucherCode { get; set; }
        public decimal Total { get; set; }

        public class OrderDetailViewModel
        {
            public int Id { get; set; }
            public ProductVariantViewModel ProductVariant { get; set; }
            public int Quantity { get; set; }
        }

        public class ProductVariantViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
            public string Version { get; set; }
            public decimal SellingPrice { get; set; }
            public string Image { get; set; }
        }
    }

    public class AddToCartResponse
    {
        public bool success { get; set; }
        public int cartCount { get; set; }
    }

    public class UpdateCartResponse
    {
        public bool success { get; set; }
        public int cartCount { get; set; }
        public decimal total { get; set; }
    }

    public class RemoveCartResponse
    {
        public bool success { get; set; }
        public int cartCount { get; set; }
        public decimal total { get; set; }
    }

    public class ApplyVoucherResponse
    {
        public bool success { get; set; }
        public decimal total { get; set; }
    }

    public class CartCountResponse
    {
        public bool success { get; set; }
        public int cartCount { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }
}