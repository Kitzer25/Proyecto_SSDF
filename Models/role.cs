using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Roles para control de acceso del personal interno. Un usuario puede tener múltiples roles via user_roles.
/// </summary>
public partial class role
{
    public Guid role_id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public bool is_active { get; set; }

    public virtual ICollection<user_role> user_roles { get; set; } = new List<user_role>();
}
