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
using Microsoft.Extensions.Logging;

namespace PhoneStoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PhoneStoreAPI");
            _logger = logger;
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
                    if (result == null || string.IsNullOrEmpty(result.Token) || result.UserId == 0)
                    {
                        _logger.LogWarning("API login trả về dữ liệu không hợp lệ cho username: {Username}.", model.Username);
                        ModelState.AddModelError(string.Empty, "Đăng nhập không thành công: Dữ liệu phản hồi không hợp lệ.");
                        return View(model);
                    }

                    // ✅ Lưu JWT vào cookie
                    HttpContext.Response.Cookies.Append("JwtToken", result.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(1)
                    });

                    // ✅ Tạo claims
                    int role = result.Role;
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                    };

                    // ✅ Đăng nhập và lưu thông tin vào session
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    HttpContext.Session.SetString("Role", role.ToString());
                    HttpContext.Session.SetString("UserId", result.UserId.ToString()); // ✅ Thêm dòng này

                    _logger.LogInformation("Đăng nhập thành công cho username: {Username}, UserId: {UserId}, Role: {Role}", model.Username, result.UserId, role);

                    switch (role)
                    {
                        case 1: return RedirectToAction("Dashboard", "Dashboard", new { area = "Admin" });
                        case 2: return RedirectToAction("Index", "Staff");
                        case 3: return RedirectToAction("Index", "Home");
                        default: return RedirectToAction("Index", "Home");
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Đăng nhập thất bại. Trạng thái: {StatusCode}, Lỗi: {ErrorContent}", response.StatusCode, errorContent);
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập cho username: {Username}", model.Username);
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi khi đăng nhập: {ex.Message}");
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
                    _logger.LogInformation("Đăng ký thành công cho username: {Username}", model.Username);
                    return RedirectToAction("Login");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Đăng ký thất bại cho username: {Username}. Mã trạng thái: {StatusCode}, Nội dung lỗi: {ErrorContent}", model.Username, response.StatusCode, errorContent);
                ModelState.AddModelError(string.Empty, $"Đăng ký không thành công: {errorContent}");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký cho username: {Username}", model.Username);
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi khi đăng ký: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("JwtToken");
            HttpContext.Session?.Clear();
            _logger.LogInformation("Đăng xuất thành công.");
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
                    _logger.LogWarning("Không tìm thấy username trong claims khi truy cập Profile.");
                    return RedirectToAction("Login");
                }

                var token = HttpContext.Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy JwtToken trong cookie khi truy cập Profile.");
                    return RedirectToAction("Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"api/Users/search?username={Uri.EscapeDataString(username)}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Không thể tải thông tin hồ sơ từ API cho username: {Username}. Mã trạng thái: {StatusCode}, Nội dung lỗi: {ErrorContent}", username, response.StatusCode, errorContent);
                    ModelState.AddModelError(string.Empty, "Không thể tải thông tin hồ sơ từ API");
                    return View(new UserViewModel());
                }

                var users = await response.Content.ReadFromJsonAsync<UserViewModel[]>();
                if (users == null || users.Length == 0)
                {
                    _logger.LogWarning("Không tìm thấy thông tin người dùng cho username: {Username}", username);
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng");
                    return View(new UserViewModel());
                }

                var userProfile = users[0];
                _logger.LogInformation("Lấy thông tin hồ sơ thành công cho username: {Username}", username);
                return View(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải hồ sơ cho username: {Username}", User?.Identity?.Name);
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                return View(new UserViewModel());
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState không hợp lệ khi cập nhật hồ sơ cho username: {Username}", model.Username);
                return View(model);
            }

            try
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy JwtToken trong cookie khi cập nhật hồ sơ.");
                    return RedirectToAction("Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PutAsJsonAsync($"api/Users/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Cập nhật hồ sơ thành công cho username: {Username}", model.Username);
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công";
                    return RedirectToAction("Profile");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Cập nhật hồ sơ thất bại cho username: {Username}. Mã trạng thái: {StatusCode}, Nội dung lỗi: {ErrorContent}", model.Username, response.StatusCode, errorContent);
                ModelState.AddModelError(string.Empty, $"Cập nhật hồ sơ không thành công: {errorContent}");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hồ sơ cho username: {Username}", model.Username);
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                return View(model);
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int Role { get; set; }
        public int UserId { get; set; }
    }
}