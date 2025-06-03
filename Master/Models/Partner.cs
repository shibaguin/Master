using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Partner
{
    public string PartnerId { get; set; } = null!;
    
    public string Id => PartnerId;

    public string PartnerType { get; set; } = null!;

    public string? PartnerName { get; set; }

    public string Director { get; set; } = null!;

    public string? Mail { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string Inn { get; set; } = null!;

    public string? Rating { get; set; }
}
