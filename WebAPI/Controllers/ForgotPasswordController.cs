using BusinessObject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;

namespace PhoneStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _forgotPasswordService;

        public ForgotPasswordController(IForgotPasswordService forgotPasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
        }

        [HttpPost("send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.EmailToSendOtp))
                return BadRequest("Dữ liệu không hợp lệ");

            var result = await _forgotPasswordService.SendOtpAsync(model.Username, model.EmailToSendOtp);
            if (!result)
                return NotFound("Không tìm thấy người dùng");

            return Ok("OTP đã được gửi");
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Otp) || string.IsNullOrEmpty(model.NewPassword))
                return BadRequest("Dữ liệu không hợp lệ");

            var result = await _forgotPasswordService.ResetPasswordAsync(model.Username, model.Otp, model.NewPassword);
            if (!result)
                return BadRequest("OTP không đúng hoặc người dùng không tồn tại");

            return Ok("Đổi mật khẩu thành công");
        }
    }
}