namespace WebMVC.Models
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public int BrandId { get; set; }
        public bool IsDeleted { get; set; }
        public List<ProductVariantViewModel> Variants { get; set; }
    }

    public class ProductVariantViewModel
    {
        public int? Id { get; set; } // Nullable to handle cases where API doesn't return Id
        public string Color { get; set; }
        public string Version { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public string Image { get; set; }
    }
}