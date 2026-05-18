using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Catálogo de formas farmacéuticas. Seed: Tableta, Cápsula, Jarabe, Suspensión, Inyectable, Crema, Gel, Parche, Gotas, Spray, Supositorio, Polvo.
/// </summary>
public partial class drug_form
{
    public Guid drug_form_id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public bool is_active { get; set; }

    public virtual ICollection<product_variant> product_variants { get; set; } = new List<product_variant>();
}
