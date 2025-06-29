using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PhoneStoreMVC.ViewModels;
using System.Text.Json;

namespace PhoneStoreMVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var username = User?.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("Username is null or empty, redirecting to Login");
                    return RedirectToAction("Login", "Account");
                }

                Console.WriteLine($"Authenticated Username: {username}");
                var url = $"api/Users/search?username={Uri.EscapeDataString(username)}";
                Console.WriteLine($"Requesting API: {url}");

                // Thêm token nếu có (dựa trên session từ Login)
                var token = HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"API Response Status: {response.StatusCode} - {response.ReasonPhrase}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response Content: {content}");

                var users = await response.Content.ReadFromJsonAsync<UserViewModel[]>();
                if (users == null || users.Length == 0)
                {
                    Console.WriteLine("No users found for the given username");
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng");
                    return View(new UserViewModel());
                }

                var userProfile = users[0];
                Console.WriteLine($"User Profile Loaded: {userProfile.Username}");
                return View(userProfile);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Lỗi kết nối đến API: {ex.Message}");
                return View(new UserViewModel());
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Lỗi phân tích dữ liệu từ API: {ex.Message}");
                return View(new UserViewModel());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi không mong muốn: {ex.Message}");
                return View(new UserViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Thêm token nếu có
                var token = HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PutAsJsonAsync($"api/Users/{model.Id}", model);
                Console.WriteLine($"API Update Response Status: {response.StatusCode} - {response.ReasonPhrase}");

                response.EnsureSuccessStatusCode();

                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Lỗi kết nối đến API: {ex.Message}");
                return View("Index", model);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Lỗi phân tích dữ liệu: {ex.Message}");
                return View("Index", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                return View("Index", model);
            }
        }
    }
}