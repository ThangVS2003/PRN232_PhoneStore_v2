using Microsoft.Extensions.Configuration;
using Repository.IRepository;
using Service.IServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        // In-memory store: email → otp
        private static ConcurrentDictionary<string, string> _otpStore = new ConcurrentDictionary<string, string>();

        public ForgotPasswordService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<bool> SendOtpAsync(string username, string emailToSendOtp)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.IsDeleted == true)
                return false;

            var otp = new Random().Next(100000, 999999).ToString();
            _otpStore[username] = otp;

            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:Port"]))
            {
                Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:From"]),
                Subject = "Mã OTP khôi phục mật khẩu",
                Body = $"Mã OTP của bạn là: {otp}",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(emailToSendOtp);

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string username, string otp, string newPassword)
        {
            if (!_otpStore.TryGetValue(username, out var storedOtp) || storedOtp != otp)
                return false;

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.IsDeleted == true)
                return false;

            user.Password = newPassword; // nên hash thực tế
            await _userRepository.UpdateAsync(user);

            _otpStore.TryRemove(username, out _);
            return true;
        }
    }
}
