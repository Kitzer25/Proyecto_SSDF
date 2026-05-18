using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Códigos de cupón asociados a una promoción. Ej: VERANO10. Tienen control de usos propio además del límite de la promoción.
/// </summary>
public partial class promotion_code
{
    public Guid code_id { get; set; }

    public Guid promotion_id { get; set; }

    /// <summary>
    /// Código que ingresa el cliente al hacer checkout. Único en todo el sistema.
    /// </summary>
    public string code { get; set; } = null!;

    public int? max_uses { get; set; }

    public int current_uses { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public virtual ICollection<order> orders { get; set; } = new List<order>();

    public virtual promotion promotion { get; set; } = null!;
}
