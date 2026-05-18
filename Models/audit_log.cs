using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Log de auditoría para acciones sensibles (cambios de precio, roles, stock). Inmutable. Usar BIGSERIAL por volumen de escritura. Considerar particionamiento por created_at cuando supere 5M filas.
/// </summary>
public partial class audit_log
{
    public long audit_log_id { get; set; }

    public string table_name { get; set; } = null!;

    /// <summary>
    /// UUID o ID del registro afectado como TEXT para compatibilidad con cualquier tipo de PK.
    /// </summary>
    public string record_id { get; set; } = null!;

    public string action { get; set; } = null!;

    /// <summary>
    /// Estado anterior del registro en JSONB. NULL en INSERT.
    /// </summary>
    public string? old_values { get; set; }

    /// <summary>
    /// Estado nuevo del registro en JSONB. NULL en DELETE.
    /// </summary>
    public string? new_values { get; set; }

    public Guid? user_id { get; set; }

    public Guid? customer_id { get; set; }

    public string? ip_address { get; set; }

    public string? user_agent { get; set; }

    public DateTime created_at { get; set; }

    public virtual customer? customer { get; set; }

    public virtual user1? user { get; set; }
}
