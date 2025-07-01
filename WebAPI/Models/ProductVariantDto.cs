namespace PhoneStoreAPI.Models
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ColorId { get; set; }
        public int? VersionId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Image { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
