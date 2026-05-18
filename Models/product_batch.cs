using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Lotes de medicamentos con trazabilidad de vencimiento. Obligatorio para regulación farmacéutica. Lógica FEFO aplicada al despachar.
/// </summary>
public partial class product_batch
{
    public Guid batch_id { get; set; }

    public Guid product_variant_id { get; set; }

    public Guid? laboratory_id { get; set; }

    /// <summary>
    /// Número de lote impreso en el empaque del fabricante.
    /// </summary>
    public string batch_number { get; set; } = null!;

    public DateOnly? manufacture_date { get; set; }

    /// <summary>
    /// Fecha de vencimiento. Nunca vender si expiration_date &lt; NOW().
    /// </summary>
    public DateOnly expiration_date { get; set; }

    public int initial_quantity { get; set; }

    public int current_quantity { get; set; }

    public string? notes { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<inventory_movement> inventory_movements { get; set; } = new List<inventory_movement>();

    public virtual laboratory? laboratory { get; set; }

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();

    public virtual product_variant product_variant { get; set; } = null!;
}
