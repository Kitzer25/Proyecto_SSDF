using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

public partial class v_customer_order_summary
{
    public Guid? customer_id { get; set; }

    public string? email { get; set; }

    public string? full_name { get; set; }

    public long? total_orders { get; set; }

    public decimal? total_spent { get; set; }

    public DateTime? last_order_at { get; set; }
}
