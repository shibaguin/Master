using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Sale
{
    public string SaleId { get; set; } = null!;

    public string? PartnerId { get; set; }

    public string? ProductId { get; set; }

    public int? Quantity { get; set; }

    public DateOnly? SaleDate { get; set; }
}
