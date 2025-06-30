using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IHttpClientFactory httpClientFactory, ILogger<UsersController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _httpClient.BaseAddress = new Uri("https://localhost:7026/api/");
            _logger = logger;
        }

        [HttpGet]
        [Route("Admin/Users")]
        public async Task<IActionResult> Index(string username = "")
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || role != "1")
            {
                _logger.LogWarning("Access denied: Role is {Role}", role);
                return RedirectToAction("AccessDenied", "Home", new { area = "" });
            }

            try
            {
                var url = string.IsNullOrEmpty(username) ? "Users" : $"Users/search?username={username}";
                _logger.LogInformation("Calling API: {Url}", url);
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed for {Url}, StatusCode: {StatusCode}", url, response.StatusCode);
                    ViewBag.SelectedUsername = username;
                    return View(new List<User>());
                }

                var users = await response.Content.ReadFromJsonAsync<List<User>>();
                ViewBag.SelectedUsername = username;
                return View(users ?? new List<User>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users for username: {Username}", username);
                ViewBag.SelectedUsername = username;
                return View(new List<User>());
            }
        }

        [HttpGet]
        [Route("Admin/Users/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || role != "1")
            {
                _logger.LogWarning("Access denied: Role is {Role}", role);
                return RedirectToAction("AccessDenied", "Home", new { area = "" });
            }

            try
            {
                var response = await _httpClient.GetAsync($"Users/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed for Users/{Id}, StatusCode: {StatusCode}", id, response.StatusCode);
                    return NotFound("Không tìm thấy người dùng");
                }

                var user = await response.Content.ReadFromJsonAsync<User>();
                if (user == null)
                {
                    _logger.LogError("User data is null for ID: {Id}", id);
                    return NotFound("Không thể lấy dữ liệu người dùng");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user data for ID: {Id}", id);
                return StatusCode(500, "Lỗi server khi lấy dữ liệu người dùng");
            }
        }

        [HttpGet]
        [Route("Admin/Users/Create")]
        public IActionResult Create()
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || role != "1")
            {
                _logger.LogWarning("Access denied: Role is {Role}", role);
                return RedirectToAction("AccessDenied", "Home", new { area = "" });
            }

            return View();
        }

        [HttpGet]
        [Route("Admin/Users/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || role != "1")
            {
                _logger.LogWarning("Access denied: Role is {Role}", role);
                return RedirectToAction("AccessDenied", "Home", new { area = "" });
            }

            try
            {
                _logger.LogInformation("Calling API: Users/{Id}", id);
                var response = await _httpClient.GetAsync($"Users/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed for Users/{Id}, StatusCode: {StatusCode}", id, response.StatusCode);
                    return NotFound("Không tìm thấy người dùng");
                }

                var user = await response.Content.ReadFromJsonAsync<User>();
                if (user == null)
                {
                    _logger.LogError("User data is null for ID: {Id}", id);
                    return NotFound("Không thể lấy dữ liệu người dùng");
                }

                _logger.LogInformation("User data for ID {Id}: Username={Username}, Name={Name}, Email={Email}, Phone={Phone}, Address={Address}, Role={Role}, Money={Money}",
                    user.Id, user.Username, user.Name, user.Email, user.Phone, user.Address, user.Role, user.Money);

                user.Password = null;
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user data for ID: {Id}", id);
                return StatusCode(500, "Lỗi server khi lấy dữ liệu người dùng");
            }
        }

        [HttpGet]
        [Route("Admin/Users/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || role != "1")
            {
                _logger.LogWarning("Access denied: Role is {Role}", role);
                return RedirectToAction("AccessDenied", "Home", new { area = "" });
            }

            try
            {
                var response = await _httpClient.GetAsync($"Users/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API call failed for Users/{Id}, StatusCode: {StatusCode}", id, response.StatusCode);
                    return NotFound("Không tìm thấy người dùng");
                }

                var user = await response.Content.ReadFromJsonAsync<User>();
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user data for ID: {Id}", id);
                return StatusCode(500, "Lỗi server khi lấy dữ liệu người dùng");
            }
        }
    }
}