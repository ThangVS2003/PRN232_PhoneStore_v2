using System.ComponentModel.DataAnnotations;

namespace PhoneStoreMVC.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public string EmailToSendOtp { get; set; }
        public string Username { get; set; }
        public string NewPassword { get; set; }
        public string Otp { get; set; }
    }
}
