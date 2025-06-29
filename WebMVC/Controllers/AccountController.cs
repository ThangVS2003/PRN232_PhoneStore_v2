using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using PhoneStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;

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

                    int role = result.Role;
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, role.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    HttpContext.Session.SetString("Role", role.ToString());
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
            HttpContext.Session?.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var username = User?.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login");
                }

                var response = await _httpClient.GetAsync($"api/Users/search?username={Uri.EscapeDataString(username)}");
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tải thông tin hồ sơ từ API");
                    return View();
                }

                var users = await response.Content.ReadFromJsonAsync<UserViewModel[]>();
                if (users == null || users.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng");
                    return View(new UserViewModel());
                }

                var userProfile = users[0];
                return View(userProfile);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                return View(new UserViewModel());
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Users/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công";
                    return RedirectToAction("Profile");
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Cập nhật hồ sơ không thành công: {errorMessage}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                return View(model);
            }
        }
    }
}