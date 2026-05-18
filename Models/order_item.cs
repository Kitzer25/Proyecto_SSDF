using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Líneas del pedido. Inmutables una vez creadas. Los snapshots garantizan integridad histórica aunque el producto cambie después.
/// </summary>
public partial class order_item
{
    public Guid order_item_id { get; set; }

    public Guid order_id { get; set; }

    public Guid product_variant_id { get; set; }

    /// <summary>
    /// Lote del que se extrajo el medicamento. Asignado al despachar según lógica FEFO.
    /// </summary>
    public Guid? batch_id { get; set; }

    public Guid? prescription_id { get; set; }

    public int quantity { get; set; }

    /// <summary>
    /// Precio unitario al momento de la compra. Snapshot inmutable.
    /// </summary>
    public decimal unit_price { get; set; }

    public decimal discount_amount { get; set; }

    public decimal subtotal { get; set; }

    /// <summary>
    /// Nombre del producto al momento de la compra.
    /// </summary>
    public string product_name_snapshot { get; set; } = null!;

    /// <summary>
    /// Descripción de la variante al momento de la compra. Ej: Tableta 500mg x30.
    /// </summary>
    public string variant_desc_snapshot { get; set; } = null!;

    /// <summary>
    /// SKU al momento de la compra para trazabilidad contable.
    /// </summary>
    public string sku_snapshot { get; set; } = null!;

    public virtual product_batch? batch { get; set; }

    public virtual order order { get; set; } = null!;

    public virtual prescription_upload? prescription { get; set; }

    public virtual product_variant product_variant { get; set; } = null!;
}
