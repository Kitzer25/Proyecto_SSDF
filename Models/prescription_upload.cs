using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Recetas médicas subidas por clientes. Obligatoria para productos con requires_prescription = TRUE. El pedido no avanza a Processing sin verificación.
/// </summary>
public partial class prescription_upload
{
    public Guid prescription_id { get; set; }

    public Guid customer_id { get; set; }

    public Guid? order_id { get; set; }

    /// <summary>
    /// URL del archivo en Supabase Storage. El bucket debe ser privado con acceso controlado.
    /// </summary>
    public string image_url { get; set; } = null!;

    public string? doctor_name { get; set; }

    public string? doctor_license { get; set; }

    public DateOnly? issued_date { get; set; }

    /// <summary>
    /// Un farmacéutico (user) debe verificar la receta antes de despachar.
    /// </summary>
    public bool is_verified { get; set; }

    public Guid? verified_by_user_id { get; set; }

    public DateTime? verified_at { get; set; }

    /// <summary>
    /// Motivo de rechazo de la receta. Visible para el cliente.
    /// </summary>
    public string? rejection_reason { get; set; }

    public string? notes { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual customer customer { get; set; } = null!;

    public virtual order? order { get; set; }

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();

    public virtual user1? verified_by_user { get; set; }
}
