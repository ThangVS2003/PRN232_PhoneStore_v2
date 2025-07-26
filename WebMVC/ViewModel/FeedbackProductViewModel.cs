using System.ComponentModel.DataAnnotations;

namespace PhoneStoreMVC.ViewModels
{
    public class FeedbackProductViewModel
    {
        [Required(ErrorMessage = "Mã đơn hàng là bắt buộc")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Mã người dùng là bắt buộc")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Vui lòng đánh giá")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận không được vượt quá 500 ký tự")]
        public string? Comment { get; set; }

        // Thuộc tính chỉ dùng để hiển thị
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
    }
}