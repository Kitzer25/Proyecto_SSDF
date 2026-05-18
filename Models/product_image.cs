using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Imágenes a nivel de producto genérico o variante específica. Solo se guarda la URL; el archivo vive en Supabase Storage o CDN externo.
/// </summary>
public partial class product_image
{
    public Guid product_image_id { get; set; }

    public Guid? product_id { get; set; }

    public Guid? product_variant_id { get; set; }

    /// <summary>
    /// URL pública del archivo. Puede ser Supabase Storage URL.
    /// </summary>
    public string image_url { get; set; } = null!;

    public string? alt_text { get; set; }

    /// <summary>
    /// Imagen principal que se muestra en el listado del catálogo.
    /// </summary>
    public bool is_main { get; set; }

    public int sort_order { get; set; }

    public DateTime created_at { get; set; }

    public virtual product? product { get; set; }

    public virtual product_variant? product_variant { get; set; }
}
