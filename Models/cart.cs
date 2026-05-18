using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Carritos de compra persistentes. customer_id NULL = carrito anónimo identificado por session_id. Al hacer login, el carrito anónimo se asocia al cliente.
/// </summary>
public partial class cart
{
    public Guid cart_id { get; set; }

    public Guid? customer_id { get; set; }

    public string? session_id { get; set; }

    public bool is_active { get; set; }

    /// <summary>
    /// Fecha de expiración del carrito. Limpiar carritos expirados con un job periódico.
    /// </summary>
    public DateTime? expires_at { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<cart_item> cart_items { get; set; } = new List<cart_item>();

    public virtual customer? customer { get; set; }
}
