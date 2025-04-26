using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string? ProductName { get; set; }

    public string? ProductTypeId { get; set; }
}
