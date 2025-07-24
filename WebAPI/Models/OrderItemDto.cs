namespace PhoneStoreAPI.Models
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string? Image { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int? ColorId { get; set; }
        public string? ColorName { get; set; }
        public int? VersionId { get; set; }
        public string? VersionName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
