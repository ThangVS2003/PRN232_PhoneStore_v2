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
using System.ComponentModel.DataAnnotations;
using WebMVC.ViewModel;

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
            {
                _logger.LogWarning("ModelState không hợp lệ khi đăng nhập với username: {Username}", model.Username);
                return View(model);
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result == null || string.IsNullOrEmpty(result.Token) || result.UserId == 0)
                    {
                        _logger.LogWarning("API login trả về dữ liệu không hợp lệ cho username: {Username}", model.Username);
                        ModelState.AddModelError(string.Empty, "Đăng nhập không thành công: Dữ liệu phản hồi không hợp lệ.");
                        return View(model);
                    }

                    // Lưu JWT vào cookie
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(result.Token);
                    var expiry = jwtToken.ValidTo;

                    HttpContext.Response.Cookies.Append("JwtToken", result.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = expiry.ToUniversalTime()
                    });

                    // Tạo claims
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
                        ExpiresUtc = expiry
                    };

                    // Đăng nhập
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    HttpContext.Session.SetString("Role", role.ToString());
                    HttpContext.Session.SetString("UserId", result.UserId.ToString());

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
            {
                _logger.LogWarning("ModelState không hợp lệ khi đăng ký với username: {Username}", model.Username);
                return View(model);
            }

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
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Không tìm thấy UserId trong claims khi truy cập Profile.");
                    return RedirectToAction("Login");
                }

                var token = HttpContext.Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Không tìm thấy JwtToken trong cookie khi truy cập Profile.");
                    return RedirectToAction("Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"api/Users/{userId}"); // Sử dụng UserId thay vì username
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Không thể tải thông tin hồ sơ từ API cho UserId: {UserId}. Mã trạng thái: {StatusCode}, Nội dung lỗi: {ErrorContent}", userId, response.StatusCode, errorContent);
                    ModelState.AddModelError(string.Empty, "Không thể tải thông tin hồ sơ từ API");
                    return View(new UserViewModel());
                }

                var userProfile = await response.Content.ReadFromJsonAsync<UserViewModel>();
                if (userProfile == null)
                {
                    _logger.LogWarning("Không tìm thấy thông tin người dùng cho UserId: {UserId}", userId);
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng");
                    return View(new UserViewModel());
                }

                _logger.LogInformation("Lấy thông tin hồ sơ thành công cho UserId: {UserId}", userId);
                return View(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải hồ sơ cho UserId: {UserId}", User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
                _logger.LogWarning("ModelState không hợp lệ khi cập nhật hồ sơ cho UserId: {UserId}", model.Id);
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
                    _logger.LogInformation("Cập nhật hồ sơ thành công cho UserId: {UserId}", model.Id);
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công";
                    return RedirectToAction("Profile");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Cập nhật hồ sơ thất bại cho UserId: {UserId}. Mã trạng thái: {StatusCode}, Nội dung lỗi: {ErrorContent}", model.Id, response.StatusCode, errorContent);
                ModelState.AddModelError(string.Empty, $"Cập nhật hồ sơ không thành công: {errorContent}");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hồ sơ cho UserId: {UserId}", model.Id);
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, string action)
        {
            if (action == "SendOtp")
            {
                ModelState.Remove(nameof(model.NewPassword));
                ModelState.Remove(nameof(model.Otp));

                if (string.IsNullOrEmpty(model.EmailToSendOtp))
                {
                    ModelState.AddModelError(nameof(model.EmailToSendOtp), "Email không được để trống");
                    return View(model);
                }

                try
                {
                    var sendModel = new { Username = model.Username, EmailToSendOtp = model.EmailToSendOtp };
                    var response = await _httpClient.PostAsJsonAsync("api/ForgotPassword/send-otp", sendModel);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Đã gửi OTP thành công";
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"Gửi OTP thất bại: {error}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi gửi OTP");
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                }

                return View(model);
            }

            if (action == "ResetPassword")
            {
                // Không kiểm tra gì, gửi luôn
                try
                {
                    var resetModel = new { Username = model.Username, Otp = model.Otp, NewPassword = model.NewPassword };
                    var response = await _httpClient.PostAsJsonAsync("api/ForgotPassword/reset-password", resetModel);
                    if (response.IsSuccessStatusCode)
                    {
                        //TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"Đổi mật khẩu thất bại: {error}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi đổi mật khẩu");
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
                }
            }

            return View(model);
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int Role { get; set; }
        public int UserId { get; set; }
    }
}