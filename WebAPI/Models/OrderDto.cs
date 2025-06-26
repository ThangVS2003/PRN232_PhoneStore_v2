namespace PhoneStoreAPI.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Status { get; set; }
        public string? ShippingAddress { get; set; }
        public int? VoucherId { get; set; }
    }
}
