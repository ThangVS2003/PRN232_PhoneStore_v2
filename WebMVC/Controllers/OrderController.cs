using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PhoneStoreMVC.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace WebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = HttpContext.Request.Cookies["JwtToken"];

            Console.WriteLine($"[DEBUG] UserId from Claims: {userId ?? "(null)"}");
            Console.WriteLine($"[DEBUG] Token from Cookie: {(string.IsNullOrEmpty(token) ? "(null)" : "[HIDDEN]")}");

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
            {
                Console.WriteLine("[WARNING] UserId không có trong claims hoặc sai định dạng.");
                ViewBag.Error = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn.";
                return View(new List<OrderDetailViewModel>());
            }

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("[DEBUG] Token added to header.");
            }
            else
            {
                Console.WriteLine("[WARNING] No token found in cookie.");
            }

            var apiUrl = $"api/orders/user/{parsedUserId}";
            Console.WriteLine($"[DEBUG] Calling API: {apiUrl}");

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                Console.WriteLine($"[DEBUG] API Status: {response.StatusCode} - {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] API Error: {error}");
                    ViewBag.Error = $"Lỗi API: {response.StatusCode} - {error}";
                    return View(new List<OrderDetailViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Raw JSON: {json}");

                var orders = JsonSerializer.Deserialize<List<OrderDetailViewModel>>(json, _jsonOptions);
                if (orders == null)
                {
                    Console.WriteLine("[WARNING] Deserialization returned null.");
                    ViewBag.Error = "Không thể phân tích dữ liệu từ API.";
                    return View(new List<OrderDetailViewModel>());
                }
                Console.WriteLine($"[DEBUG] Orders parsed: {orders.Count}");

                return View(orders);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[ERROR] HttpRequestException: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi khi kết nối đến API.";
                return View(new List<OrderDetailViewModel>());
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ERROR] JsonException: {ex.Message}");
                ViewBag.Error = "Lỗi phân tích dữ liệu từ API.";
                return View(new List<OrderDetailViewModel>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected Exception: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi không mong muốn.";
                return View(new List<OrderDetailViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Console.WriteLine($"[DEBUG] Fetching order detail for ID: {id}");

            var token = HttpContext.Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("[DEBUG] Token added to header for detail request.");
            }
            else
            {
                Console.WriteLine("[WARNING] No token found in cookie for detail request.");
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/orders/details/{id}");
                Console.WriteLine($"[DEBUG] Detail API Status: {response.StatusCode} - {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Detail API Error: {error}");
                    ViewBag.Error = "Không tìm thấy chi tiết đơn hàng.";
                    return RedirectToAction("History");
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Order Detail JSON: {json}");

                var orderDetail = JsonSerializer.Deserialize<OrderDetailViewModel>(json, _jsonOptions);
                if (orderDetail == null)
                {
                    Console.WriteLine("[WARNING] Deserialization returned null for order detail.");
                    ViewBag.Error = "Không thể phân tích chi tiết đơn hàng.";
                    return RedirectToAction("History");
                }
                return View(orderDetail);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[ERROR] HttpRequestException in detail: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi khi kết nối đến API.";
                return RedirectToAction("History");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ERROR] JsonException in detail: {ex.Message}");
                ViewBag.Error = "Lỗi phân tích dữ liệu chi tiết đơn hàng.";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected Exception in detail: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi không mong muốn khi tải chi tiết đơn hàng.";
                return RedirectToAction("History");
            }
        }
    }
}