using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Estado actual del stock por variante. Permite consulta O(1) de disponibilidad sin sumar movimientos históricos.
/// </summary>
public partial class inventory
{
    public Guid inventory_id { get; set; }

    public Guid product_variant_id { get; set; }

    /// <summary>
    /// Stock físico real en almacén.
    /// </summary>
    public int quantity_on_hand { get; set; }

    /// <summary>
    /// Stock reservado en pedidos confirmados pero no despachados aún.
    /// </summary>
    public int reserved_quantity { get; set; }

    /// <summary>
    /// Umbral de alerta de bajo stock. Genera notificación cuando quantity_on_hand &lt;= min_stock_level.
    /// </summary>
    public int min_stock_level { get; set; }

    public int? max_stock_level { get; set; }

    public DateTime last_updated_at { get; set; }

    public virtual product_variant product_variant { get; set; } = null!;
}
