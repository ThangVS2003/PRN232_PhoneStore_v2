using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Serial
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string? Status { get; set; }

    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
