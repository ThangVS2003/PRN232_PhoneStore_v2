using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public string Content { get; set; } = null!;

    public int? ReplyId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Comment> InverseReply { get; set; } = new List<Comment>();

    public virtual Product Product { get; set; } = null!;

    public virtual Comment? Reply { get; set; }

    public virtual User User { get; set; } = null!;
}
