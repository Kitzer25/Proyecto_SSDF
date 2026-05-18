using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Cabecera del pedido. Documento contable inmutable. Los montos son snapshot al momento de confirmar y NUNCA se recalculan.
/// </summary>
public partial class order
{
    public Guid order_id { get; set; }

    /// <summary>
    /// Identificador legible para el cliente. Ej: ORD-2024-000001. Generado por la aplicación con una secuencia.
    /// </summary>
    public string order_number { get; set; } = null!;

    public Guid customer_id { get; set; }

    public Guid order_status_id { get; set; }

    public Guid shipping_address_id { get; set; }

    public Guid? promotion_code_id { get; set; }

    public decimal subtotal { get; set; }

    public decimal tax_amount { get; set; }

    public decimal shipping_cost { get; set; }

    public decimal discount_amount { get; set; }

    /// <summary>
    /// subtotal + tax_amount + shipping_cost - discount_amount. Calculado y fijado al confirmar el pedido.
    /// </summary>
    public decimal total { get; set; }

    public string? customer_notes { get; set; }

    public string? internal_notes { get; set; }

    public DateOnly? estimated_delivery { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual customer customer { get; set; } = null!;

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();

    public virtual ICollection<order_payment> order_payments { get; set; } = new List<order_payment>();

    public virtual order_status order_status { get; set; } = null!;

    public virtual ICollection<order_status_history> order_status_histories { get; set; } = new List<order_status_history>();

    public virtual ICollection<prescription_upload> prescription_uploads { get; set; } = new List<prescription_upload>();

    public virtual promotion_code? promotion_code { get; set; }

    public virtual customer_address shipping_address { get; set; } = null!;
}
