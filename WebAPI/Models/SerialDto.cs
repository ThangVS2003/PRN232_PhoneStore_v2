namespace PhoneStoreAPI.Models
{
    public class SerialDto
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string SerialNumber { get; set; } = null!;
        public string? Status { get; set; }
    }

}
