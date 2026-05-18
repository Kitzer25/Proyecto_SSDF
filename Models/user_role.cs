using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Tabla puente N:M entre users y roles. Un usuario puede tener múltiples roles. Registra quién asignó el rol.
/// </summary>
public partial class user_role
{
    public Guid user_id { get; set; }

    public Guid role_id { get; set; }

    public DateTime assigned_at { get; set; }

    public Guid? assigned_by_user_id { get; set; }

    public virtual user1? assigned_by_user { get; set; }

    public virtual role role { get; set; } = null!;

    public virtual user1 user { get; set; } = null!;
}
