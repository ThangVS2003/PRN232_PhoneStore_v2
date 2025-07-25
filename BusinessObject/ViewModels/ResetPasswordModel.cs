using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.ViewModels
{
    public class SendOtpModel
    {
        public string Username { get; set; }
        public string EmailToSendOtp { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Username { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}
