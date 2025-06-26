namespace PhoneStoreAPI.Models
{
    public class DepositHistoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
    }
}
