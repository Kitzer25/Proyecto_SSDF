using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Personal interno: administradores, farmacéuticos, vendedores. Separado de customers. Accede al backoffice, no al eCommerce público.
/// </summary>
public partial class user1
{
    public Guid user_id { get; set; }

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public string first_name { get; set; } = null!;

    public string last_name { get; set; } = null!;

    public string? phone { get; set; }

    public bool is_active { get; set; }

    public DateTime? last_login_at { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public Guid? auth_user_id { get; set; }

    public virtual ICollection<audit_log> audit_logs { get; set; } = new List<audit_log>();

    public virtual user? auth_user { get; set; }

    public virtual ICollection<inventory_movement> inventory_movements { get; set; } = new List<inventory_movement>();

    public virtual ICollection<order_status_history> order_status_histories { get; set; } = new List<order_status_history>();

    public virtual ICollection<prescription_upload> prescription_uploads { get; set; } = new List<prescription_upload>();

    public virtual ICollection<user_role> user_roleassigned_by_users { get; set; } = new List<user_role>();

    public virtual ICollection<user_role> user_roleusers { get; set; } = new List<user_role>();
}
