using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public string? ShippingAddress { get; set; }

    public int? VoucherId { get; set; }

    public virtual ICollection<FeedbackOrder> FeedbackOrders { get; set; } = new List<FeedbackOrder>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}
