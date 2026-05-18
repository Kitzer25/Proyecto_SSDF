using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Compradores del eCommerce. Separados de users (personal interno) por tener flujos, permisos y datos distintos.
/// </summary>
public partial class customer
{
    public Guid customer_id { get; set; }

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public string first_name { get; set; } = null!;

    public string last_name { get; set; } = null!;

    public string? phone { get; set; }

    public DateOnly? date_of_birth { get; set; }

    /// <summary>
    /// DNI, RUC, pasaporte u otro documento de identidad.
    /// </summary>
    public string? document_number { get; set; }

    public bool is_email_verified { get; set; }

    public bool is_active { get; set; }

    public DateTime? last_login_at { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    /// <summary>
    /// Soft delete. Preserva historial de pedidos del cliente.
    /// </summary>
    public DateTime? deleted_at { get; set; }

    public Guid? auth_user_id { get; set; }

    public virtual ICollection<audit_log> audit_logs { get; set; } = new List<audit_log>();

    public virtual user? auth_user { get; set; }

    public virtual ICollection<cart> carts { get; set; } = new List<cart>();

    public virtual ICollection<customer_address> customer_addresses { get; set; } = new List<customer_address>();

    public virtual ICollection<order> orders { get; set; } = new List<order>();

    public virtual ICollection<prescription_upload> prescription_uploads { get; set; } = new List<prescription_upload>();
}
