using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class ProductMaterial
{
    public string ProductId { get; set; } = null!;

    public string MaterialId { get; set; } = null!;

    public decimal? QuantityRequired { get; set; }
}
