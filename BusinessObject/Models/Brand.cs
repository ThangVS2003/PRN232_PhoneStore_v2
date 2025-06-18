using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    [JsonIgnore]

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
