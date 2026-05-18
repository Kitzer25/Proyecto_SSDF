using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Líneas del carrito. Una por variante. El precio se recalcula al hacer checkout; unit_price_snapshot es solo referencia.
/// </summary>
public partial class cart_item
{
    public Guid cart_item_id { get; set; }

    public Guid cart_id { get; set; }

    public Guid product_variant_id { get; set; }

    public int quantity { get; set; }

    /// <summary>
    /// Precio al momento de agregar al carrito. El precio real de venta se toma de product_variants.price al confirmar.
    /// </summary>
    public decimal? unit_price_snapshot { get; set; }

    public DateTime added_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual cart cart { get; set; } = null!;

    public virtual product_variant product_variant { get; set; } = null!;
}
