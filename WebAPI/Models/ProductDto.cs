namespace PhoneStoreAPI.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? MainImage { get; set; }
        public string? BrandName { get; set; }
    }

}
