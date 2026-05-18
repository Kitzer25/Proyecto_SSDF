using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Métodos de pago habilitados. is_online diferencia pagos digitales de presenciales.
/// </summary>
public partial class payment_method
{
    public Guid payment_method_id { get; set; }

    public string name { get; set; } = null!;

    public bool is_online { get; set; }

    public bool is_active { get; set; }

    public virtual ICollection<order_payment> order_payments { get; set; } = new List<order_payment>();
}
