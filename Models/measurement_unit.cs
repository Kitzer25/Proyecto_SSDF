using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Unidades de concentración y volumen. Seed: mg, ml, g, mcg, UI, %.
/// </summary>
public partial class measurement_unit
{
    public Guid unit_id { get; set; }

    public string name { get; set; } = null!;

    public string symbol { get; set; } = null!;

    public bool is_active { get; set; }

    public virtual ICollection<product_variant> product_variants { get; set; } = new List<product_variant>();
}
