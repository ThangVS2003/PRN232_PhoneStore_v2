namespace PhoneStoreAPI.Models
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; } = null!;
        public int? ReplyId { get; set; }
    }
}

