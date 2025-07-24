using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Authorize(Roles = "2")]
    [Route("Orders")]
    public class OrdersController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                int pageSize = 5;
                var response = await _httpClient.GetAsync($"orders?isPaging=true&page={page}&pageSize={pageSize}");
                if (!response.IsSuccessStatusCode)
                    return View("Error");

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);

                var ordersJson = json.RootElement.GetProperty("data").GetRawText();
                var orders = JsonSerializer.Deserialize<List<OrderViewModel>>(ordersJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                int totalItems = json.RootElement.GetProperty("totalItems").GetInt32();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View("~/Views/Staff/Orders/Index.cshtml", orders);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string name)
        {
            try
            {
                HttpResponseMessage response;

                if (string.IsNullOrWhiteSpace(name))
                {
                    response = await _httpClient.GetAsync("orders");
                }
                else
                {
                    response = await _httpClient.GetAsync($"orders/search?name={name}");
                }

                var orders = new List<OrderViewModel>();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    orders = JsonSerializer.Deserialize<List<OrderViewModel>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    orders = new List<OrderViewModel>();
                }
                else
                {
                    return View("Error");
                }

                ViewBag.SelectedName = name;
                return View("~/Views/Staff/Orders/Index.cshtml", orders);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderViewModel dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("ID không khớp.");

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"orders/{id}", content);

                if (response.IsSuccessStatusCode)
                    return NoContent();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi cập nhật đơn hàng: " + ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"orders/{id}");

                if (response.IsSuccessStatusCode)
                    return Ok();

                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa đơn hàng: " + ex.Message);
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Gọi API lấy order chi tiết (có cả items)
                var orderResponse = await _httpClient.GetAsync($"orders/{id}");
                if (!orderResponse.IsSuccessStatusCode)
                    return NotFound();

                var orderContent = await orderResponse.Content.ReadAsStringAsync();
                var order = JsonSerializer.Deserialize<OrderViewModel>(orderContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Gọi API lấy feedback của order
                try
                {
                    var feedbackResponse = await _httpClient.GetAsync($"feedbackorders/order/{id}");
                    if (feedbackResponse.IsSuccessStatusCode)
                    {
                        var feedbackContent = await feedbackResponse.Content.ReadAsStringAsync();
                        var feedbacks = JsonSerializer.Deserialize<List<OrderFeedbackViewModel>>(feedbackContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        order.Feedbacks = feedbacks ?? new List<OrderFeedbackViewModel>();
                    }
                }
                catch
                {
                    order.Feedbacks = new List<OrderFeedbackViewModel>();
                }

                return View("~/Views/Staff/Orders/Details.cshtml", order);
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
