using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IServices
{
    public interface IForgotPasswordService
    {
        Task<bool> SendOtpAsync(string username, string emailToSendOtp);
        Task<bool> ResetPasswordAsync(string username, string otp, string newPassword);
    }
}
