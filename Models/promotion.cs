using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Descuentos y promociones con reglas de vigencia, límite de uso y monto mínimo de pedido.
/// </summary>
public partial class promotion
{
    public Guid promotion_id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public Guid discount_type_id { get; set; }

    /// <summary>
    /// Valor del descuento. Interpretar según discount_type_id: porcentaje (10 = 10%) o monto fijo (10 = S/.10).
    /// </summary>
    public decimal discount_value { get; set; }

    public decimal? min_order_amount { get; set; }

    /// <summary>
    /// Tope máximo de descuento en monto fijo. Útil para descuentos porcentuales con límite.
    /// </summary>
    public decimal? max_discount_amount { get; set; }

    public int? max_uses { get; set; }

    public int current_uses { get; set; }

    /// <summary>
    /// TRUE = aplica a todo el catálogo. FALSE = solo a variantes listadas en product_promotions.
    /// </summary>
    public bool applies_to_all { get; set; }

    public DateTime start_date { get; set; }

    public DateTime? end_date { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual discount_type discount_type { get; set; } = null!;

    public virtual ICollection<promotion_code> promotion_codes { get; set; } = new List<promotion_code>();

    public virtual ICollection<product_variant> product_variants { get; set; } = new List<product_variant>();
}
