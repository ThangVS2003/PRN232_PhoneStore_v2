namespace PhoneStoreAPI.Models
{
    public class FeedbackOrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}

