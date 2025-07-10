using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PhoneStoreMVC.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [RegularExpression(@"^[a-zA-ZÀ-Ỹà-ỹ\s]+$", ErrorMessage = "Họ và tên không được chứa số hoặc ký tự đặc biệt")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải là 10 số và không chứa ký tự đặc biệt")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string Phone { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]
        public string Password { get; set; }

        public int Role { get; set; }

        public decimal Money { get; set; }
    }
}