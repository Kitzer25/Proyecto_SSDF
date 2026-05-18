using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Trazabilidad completa de cambios de estado del pedido. Inmutable. orders.order_status_id siempre refleja el estado actual.
/// </summary>
public partial class order_status_history
{
    public Guid history_id { get; set; }

    public Guid order_id { get; set; }

    public Guid order_status_id { get; set; }

    /// <summary>
    /// Usuario que realizó el cambio. NULL si fue un proceso automático del sistema.
    /// </summary>
    public Guid? changed_by_user_id { get; set; }

    public string? notes { get; set; }

    public DateTime created_at { get; set; }

    public virtual user1? changed_by_user { get; set; }

    public virtual order order { get; set; } = null!;

    public virtual order_status order_status { get; set; } = null!;
}
