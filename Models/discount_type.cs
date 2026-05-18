using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Determina cómo interpretar el campo discount_value en promotions. Seed: Percentage, FixedAmount.
/// </summary>
public partial class discount_type
{
    public Guid discount_type_id { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<promotion> promotions { get; set; } = new List<promotion>();
}
