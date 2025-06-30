namespace WebMVC.Models
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public bool IsDeleted { get; set; }
        public List<ProductVariantViewModel> Variants { get; set; }
        public List<ProductFeedbackViewModel> Feedbacks { get; set; }
    }

    public class ProductVariantViewModel
    {
        public int? Id { get; set; }
        public string Color { get; set; }
        public string Version { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ProductFeedbackViewModel
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UserName { get; set; }
    }

    public class BrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductCreateViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public string BrandName { get; set; }
    }

    public class ProductUpdateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? MainImage { get; set; }
        public string? BrandName { get; set; }
    }
}