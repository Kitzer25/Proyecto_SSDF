using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Presentación específica y vendible de un medicamento. Ej: Paracetamol 500mg x30 tabletas. Todo lo operativo (precio, stock, SKU) vive aquí.
/// </summary>
public partial class product_variant
{
    public Guid product_variant_id { get; set; }

    public Guid product_id { get; set; }

    public Guid drug_form_id { get; set; }

    public Guid? unit_id { get; set; }

    /// <summary>
    /// Valor numérico de la concentración. La unidad se define en unit_id.
    /// </summary>
    public decimal? concentration { get; set; }

    /// <summary>
    /// Unidades por empaque. Ej: 30 (tabletas por caja).
    /// </summary>
    public int package_size { get; set; }

    public string? package_description { get; set; }

    public string sku { get; set; } = null!;

    public string? barcode { get; set; }

    public decimal price { get; set; }

    /// <summary>
    /// Precio original antes del descuento. Se muestra tachado en el eCommerce.
    /// </summary>
    public decimal? compare_at_price { get; set; }

    public int sort_order { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    /// <summary>
    /// Soft delete. Las variantes no se borran para preservar historial de pedidos.
    /// </summary>
    public DateTime? deleted_at { get; set; }

    public virtual ICollection<cart_item> cart_items { get; set; } = new List<cart_item>();

    public virtual drug_form drug_form { get; set; } = null!;

    public virtual inventory? inventory { get; set; }

    public virtual ICollection<inventory_movement> inventory_movements { get; set; } = new List<inventory_movement>();

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();

    public virtual product product { get; set; } = null!;

    public virtual ICollection<product_batch> product_batches { get; set; } = new List<product_batch>();

    public virtual ICollection<product_image> product_images { get; set; } = new List<product_image>();

    public virtual measurement_unit? unit { get; set; }

    public virtual ICollection<promotion> promotions { get; set; } = new List<promotion>();
}
