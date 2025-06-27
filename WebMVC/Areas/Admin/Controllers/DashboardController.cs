// WebMVC/Areas/Admin/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;
using BusinessObject.ViewModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/"); // Thay bằng URL API thực tế
        }

        [HttpGet]
        [Route("Admin/Dashboard")]
        public async Task<IActionResult> Dashboard(int? top = 5)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || role != "1")
            {
                return RedirectToAction("AccessDenied", "Home", new { area = "" });
            }

            var url = $"Dashboard?top={top}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<DashboardViewModel>();
                return View(data);
            }
            return View(new DashboardViewModel());
        }

        [HttpGet]
        [Route("Admin/Dashboard/GetProfit")]
        public async Task<IActionResult> GetProfit(string startDate, string endDate)
        {
            var url = $"Dashboard/GetProfit?startDate={startDate}&endDate={endDate}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<dynamic>();
                return Json(data);
            }
            return Json(new { success = false, profit = 0m });
        }
    }
}