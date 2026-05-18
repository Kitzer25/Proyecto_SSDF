using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

public partial class v_available_stock
{
    public Guid? product_variant_id { get; set; }

    public string? sku { get; set; }

    public string? product_name { get; set; }

    public string? drug_form { get; set; }

    public decimal? concentration { get; set; }

    public string? unit { get; set; }

    public int? quantity_on_hand { get; set; }

    public int? reserved_quantity { get; set; }

    public int? available_quantity { get; set; }

    public int? min_stock_level { get; set; }

    public bool? is_low_stock { get; set; }
}
