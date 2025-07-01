using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PhoneStoreMVC.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;

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
            var userIdStr = HttpContext.Session.GetString("UserId");
            var token = HttpContext.Session.GetString("Token");

            Console.WriteLine($"[DEBUG] Session UserId: {userIdStr}");
            Console.WriteLine($"[DEBUG] Session Token: {(string.IsNullOrEmpty(token) ? "(null)" : "[HIDDEN]")}");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Console.WriteLine("[WARNING] UserId không có trong session hoặc sai định dạng.");
                ViewBag.Error = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn.";
                return View(new List<OrderDetailViewModel>());
            }

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var apiUrl = $"api/orders/user/{userId}";
            Console.WriteLine($"[DEBUG] Gọi API: {apiUrl}");

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                Console.WriteLine($"[DEBUG] API Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] API Error: {error}");
                    ViewBag.Error = error;
                    return View(new List<OrderDetailViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Raw JSON: {json}");

                var orders = JsonSerializer.Deserialize<List<OrderDetailViewModel>>(json, _jsonOptions);
                Console.WriteLine($"[DEBUG] Orders parsed: {orders?.Count ?? 0}");

                return View(orders ?? new List<OrderDetailViewModel>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi khi lấy danh sách đơn hàng.";
                return View(new List<OrderDetailViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Console.WriteLine($"[DEBUG] Lấy chi tiết đơn hàng ID: {id}");

            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/orders/details/{id}");
                Console.WriteLine($"[DEBUG] Detail API Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Detail API error: {error}");
                    ViewBag.Error = "Không tìm thấy chi tiết đơn hàng.";
                    return RedirectToAction("History");
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Order Detail JSON: {json}");

                var orderDetail = JsonSerializer.Deserialize<OrderDetailViewModel>(json, _jsonOptions);
                return View(orderDetail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception trong chi tiết đơn hàng: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi khi tải thông tin chi tiết đơn hàng.";
                return RedirectToAction("History");
            }
        }
    }
}
