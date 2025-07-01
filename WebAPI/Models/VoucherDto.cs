namespace PhoneStoreAPI.Models
{
    public class VoucherDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public string? DiscountType { get; set; }
        public decimal? MinOrderValue { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool? IsActive { get; set; }
        public string? ApplyType { get; set; }
        public string? Description { get; set; }
    }
}

