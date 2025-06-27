// PhoneStoreMVC/Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using PhoneStoreMVC.ViewModels;

namespace PhoneStoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(result.Token);

                    // Sử dụng role từ response body thay vì từ token
                    int role = result.Role; // Lấy role từ response (1)
                    Console.WriteLine($"Role from response: {role}"); // Debug giá trị role

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, role.ToString()) // Sử dụng role từ response
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Lưu vai trò vào session
                    HttpContext.Session.SetString("Role", role.ToString());
                    Console.WriteLine($"Session Role set to: {role}"); // Debug session

                    switch (role)
                    {
                        case 1: return RedirectToAction("Dashboard", "Dashboard", new { area = "Admin" });
                        case 2: return RedirectToAction("Index", "Staff");
                        case 3: return RedirectToAction("Index", "Home");
                        default: return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Login: {ex.Message}"); // Debug lỗi cụ thể
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng nhập: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/register", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError(string.Empty, "Đăng ký không thành công");
                return View(model);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng ký");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (HttpContext.Session != null) // Kiểm tra session trước khi xóa
            {
                HttpContext.Session.Clear();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}