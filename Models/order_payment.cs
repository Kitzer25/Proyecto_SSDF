using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Registros de pago por pedido. Permite múltiples intentos y pagos parciales. payment_status maneja el ciclo de vida del pago.
/// </summary>
public partial class order_payment
{
    public Guid payment_id { get; set; }

    public Guid order_id { get; set; }

    public Guid payment_method_id { get; set; }

    public decimal amount { get; set; }

    /// <summary>
    /// Referencia externa del pago. Ej: código de operación Yape, número de transferencia bancaria.
    /// </summary>
    public string? transaction_reference { get; set; }

    public string payment_status { get; set; } = null!;

    public DateTime? paid_at { get; set; }

    public string? notes { get; set; }

    public DateTime created_at { get; set; }

    public virtual order order { get; set; } = null!;

    public virtual payment_method payment_method { get; set; } = null!;
}
