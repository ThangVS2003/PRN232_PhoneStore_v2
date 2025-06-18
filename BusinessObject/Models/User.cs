using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class User
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

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<DepositHistory> DepositHistories { get; set; } = new List<DepositHistory>();

    public virtual ICollection<FeedbackOrder> FeedbackOrders { get; set; } = new List<FeedbackOrder>();

    public virtual ICollection<FeedbackProduct> FeedbackProducts { get; set; } = new List<FeedbackProduct>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
