using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Estados del ciclo de vida de un pedido. El historial de cambios se guarda en order_status_history.
/// </summary>
public partial class order_status
{
    public Guid order_status_id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int sort_order { get; set; }

    public bool is_active { get; set; }

    public virtual ICollection<order_status_history> order_status_histories { get; set; } = new List<order_status_history>();

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
