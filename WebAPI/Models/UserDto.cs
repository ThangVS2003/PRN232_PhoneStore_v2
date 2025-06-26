namespace PhoneStoreAPI.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Password { get; set; } = null!;
        public int? Role { get; set; }
        public decimal? Money { get; set; }
    }
}
