namespace PhoneStoreAPI.Models
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? MainImage { get; set; }
        public string BrandName { get; set; } = null!;
    }
}
