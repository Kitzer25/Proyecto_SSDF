using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Clasificación jerárquica de medicamentos. Soporta subcategorías via auto-referencia.
/// </summary>
public partial class category
{
    public Guid category_id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    /// <summary>
    /// NULL = categoría raíz. Permite un nivel de subcategorías.
    /// </summary>
    public Guid? parent_category_id { get; set; }

    /// <summary>
    /// Identificador URL-friendly único. Ej: analgesicos, antibioticos.
    /// </summary>
    public string slug { get; set; } = null!;

    public int sort_order { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<category> Inverseparent_category { get; set; } = new List<category>();

    public virtual category? parent_category { get; set; }

    public virtual ICollection<product> products { get; set; } = new List<product>();
}
