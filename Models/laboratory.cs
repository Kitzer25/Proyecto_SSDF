using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Fabricantes de medicamentos. Un laboratorio puede fabricar múltiples productos. Separado para evitar redundancia y permitir filtrado por marca.
/// </summary>
public partial class laboratory
{
    public Guid laboratory_id { get; set; }

    public string name { get; set; } = null!;

    public string? country_of_origin { get; set; }

    public string? contact_email { get; set; }

    public string? phone { get; set; }

    public string? website { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<product_batch> product_batches { get; set; } = new List<product_batch>();

    public virtual ICollection<product> products { get; set; } = new List<product>();
}
