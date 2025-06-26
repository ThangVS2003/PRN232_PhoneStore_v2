// PhoneStoreAPI/Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid || model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Dữ liệu đầu vào không hợp lệ");

            var token = await _authService.AuthenticateAsync(model.Username, model.Password);
            if (token == null)
                return Unauthorized("Thông tin đăng nhập không đúng");

            var user = await _userService.GetByUsernameAsync(model.Username);
            return Ok(new { Token = token, Role = user.Role });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid || model == null)
                return BadRequest("Dữ liệu đầu vào không hợp lệ");

            var existingUser = await _userService.GetByUsernameAsync(model.Username);
            if (existingUser != null)
                return BadRequest("Tên người dùng đã tồn tại");

            var user = new User
            {
                Username = model.Username,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                Password = model.Password, // Nên mã hóa mật khẩu trong thực tế
                Role = 3, // Mặc định là User
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _userService.AddAsync(user);
            return Ok("Đăng ký thành công");
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
    }
}