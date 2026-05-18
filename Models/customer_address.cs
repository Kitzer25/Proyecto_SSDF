using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Direcciones de envío guardadas por el cliente. Un cliente puede tener varias; is_default indica la preferida.
/// </summary>
public partial class customer_address
{
    public Guid address_id { get; set; }

    public Guid customer_id { get; set; }

    /// <summary>
    /// Etiqueta descriptiva elegida por el cliente. Ej: Casa, Trabajo, Farmacia.
    /// </summary>
    public string? label { get; set; }

    public string? recipient_name { get; set; }

    public string street { get; set; } = null!;

    public string? district { get; set; }

    public string city { get; set; } = null!;

    public string? state { get; set; }

    public string? postal_code { get; set; }

    public string country { get; set; } = null!;

    public string? phone { get; set; }

    /// <summary>
    /// Solo debe haber una dirección por defecto por cliente. Validar en aplicación.
    /// </summary>
    public bool is_default { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual customer customer { get; set; } = null!;

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
