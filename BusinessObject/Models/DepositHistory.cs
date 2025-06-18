using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class DepositHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? DepositDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
