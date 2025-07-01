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
        public int ColorId { get; set; }
        public int VersionId { get; set; }
    }

    public class ProductFeedbackViewModel
    {
        public int Id { get; set; }
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

    public class ProductVariantCreateViewModel
    {
        public int ProductId { get; set; }
        public string Color { get; set; } = null!;
        public string Version { get; set; } = null!;
        public decimal OriginalPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public string Image { get; set; } = "";
    }

    public class SerialViewModel
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string SerialNumber { get; set; } = "";
        public string? Status { get; set; }
    }

    public class ColorViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class VersionViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class VoucherViewModel
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