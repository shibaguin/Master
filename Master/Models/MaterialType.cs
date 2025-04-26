using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class MaterialType
{
    public string MaterialId { get; set; } = null!;

    public string? MaterialType1 { get; set; }

    public decimal? RejectRate { get; set; }
}
