using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Historial auditado de todos los cambios de stock. Inmutable. Permite reconciliar discrepancias y generar reportes de entradas/salidas.
/// </summary>
public partial class inventory_movement
{
    public Guid movement_id { get; set; }

    public Guid product_variant_id { get; set; }

    public Guid? batch_id { get; set; }

    public Guid movement_type_id { get; set; }

    public Guid? user_id { get; set; }

    /// <summary>
    /// Cantidad siempre positiva. La dirección (entrada/salida) la determina inventory_movement_types.direction.
    /// </summary>
    public int quantity { get; set; }

    /// <summary>
    /// Tipo de la entidad que originó el movimiento.
    /// </summary>
    public string? reference_type { get; set; }

    /// <summary>
    /// UUID de la entidad originadora (order_id, batch_id, etc.).
    /// </summary>
    public Guid? reference_id { get; set; }

    public string? notes { get; set; }

    public DateTime created_at { get; set; }

    public virtual product_batch? batch { get; set; }

    public virtual inventory_movement_type movement_type { get; set; } = null!;

    public virtual product_variant product_variant { get; set; } = null!;

    public virtual user1? user { get; set; }
}
