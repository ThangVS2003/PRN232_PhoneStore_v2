using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using WebMVC.Models;
using PhoneStoreMVC.ViewModels;
using Microsoft.Extensions.Logging; // Thêm để sử dụng ILogger

namespace WebMVC.Controllers
{
    public class FeedbackProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FeedbackProductController> _logger; // Thêm ILogger

        public FeedbackProductController(IHttpClientFactory httpClientFactory, ILogger<FeedbackProductController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _logger = logger; // Khởi tạo ILogger
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FeedbackProduct(int orderId, int productId)
        {
            _logger.LogInformation("FeedbackProduct GET called with orderId: {OrderId}, productId: {ProductId}", orderId, productId);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                _logger.LogWarning("UserId is 0 or not authenticated");
                return RedirectToAction("Login", "Account"); // Giả định có trang đăng nhập
            }
            _logger.LogInformation("UserId retrieved: {UserId}", userId);

            var productResponse = await _httpClient.GetAsync($"api/Products/{productId}");
            _logger.LogInformation("API response status for productId {ProductId}: {StatusCode}", productId, (int)productResponse.StatusCode);
            if (!productResponse.IsSuccessStatusCode)
            {
                _logger.LogError("API call failed for productId {ProductId} with status {StatusCode}", productId, (int)productResponse.StatusCode);
                return NotFound($"Không tìm thấy sản phẩm với productId: {productId}");
            }

            var productJson = await productResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("API response JSON: {ProductJson}", productJson);
            var product = JsonSerializer.Deserialize<ProductDetailViewModel>(productJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (product == null)
            {
                _logger.LogError("Deserialization failed or product is null for productId: {ProductId}", productId);
                return NotFound("Không thể deserialize dữ liệu sản phẩm");
            }
            _logger.LogInformation("Product deserialized: Name={Name}, MainImage={MainImage}", product.Name, product.MainImage);

            var model = new FeedbackProductViewModel
            {
                OrderId = orderId,
                ProductId = productId,
                UserId = userId,
                ProductName = product?.Name,
                ProductImage = product?.MainImage
            };
            _logger.LogInformation("Model created: ProductName={ProductName}, ProductImage={ProductImage}", model.ProductName, model.ProductImage);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FeedbackProduct(FeedbackProductViewModel model)
        {
            _logger.LogInformation("FeedbackProduct POST called with model: OrderId={OrderId}, ProductId={ProductId}, UserId={UserId}, Rating={Rating}, Comment={Comment}",
                model.OrderId, model.ProductId, model.UserId, model.Rating, model.Comment);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                }

                var productResponse = await _httpClient.GetAsync($"api/Products/{model.ProductId}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed for productId {ProductId} with status {StatusCode}", model.ProductId, (int)productResponse.StatusCode);
                    return NotFound("Không tìm thấy sản phẩm");
                }

                var productJson = await productResponse.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductDetailViewModel>(productJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (product == null)
                {
                    _logger.LogError("Deserialization failed or product is null for productId: {ProductId}", model.ProductId);
                }
                else
                {
                    model.ProductName = product.Name;
                    model.ProductImage = product.MainImage;
                    _logger.LogInformation("Repopulated model: ProductName={ProductName}, ProductImage={ProductImage}", model.ProductName, model.ProductImage);
                }

                return View(model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId != model.UserId)
            {
                _logger.LogError("UserId mismatch: Expected {Expected}, Got {Actual}", model.UserId, userId);
                return Unauthorized("Mã người dùng không khớp");
            }

            var feedbackPayload = new
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment
            };
            var json = JsonSerializer.Serialize(feedbackPayload);
            _logger.LogInformation("Feedback payload: {Json}", json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/FeedbackProducts", content);
            _logger.LogInformation("API POST response status: {StatusCode}", (int)response.StatusCode);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API POST failed with status {StatusCode}", (int)response.StatusCode);
                ModelState.AddModelError("", "Gửi phản hồi thất bại. Vui lòng thử lại.");
                var productResponse = await _httpClient.GetAsync($"api/Products/{model.ProductId}");
                if (productResponse.IsSuccessStatusCode)
                {
                    var productJson = await productResponse.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductDetailViewModel>(productJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    model.ProductName = product?.Name;
                    model.ProductImage = product?.MainImage;
                    _logger.LogInformation("Repopulated model after error: ProductName={ProductName}, ProductImage={ProductImage}", model.ProductName, model.ProductImage);
                }
                return View(model);
            }

            _logger.LogInformation("Feedback submitted successfully, redirecting to History");
            return RedirectToAction("History", "Order");
        }
    }
}