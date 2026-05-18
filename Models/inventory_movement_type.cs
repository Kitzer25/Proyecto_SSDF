using System;
using System.Collections.Generic;

namespace SistemaSSDF.Models;

/// <summary>
/// Clasifica cada movimiento de inventario y su dirección. La cantidad en inventory_movements siempre es positiva.
/// </summary>
public partial class inventory_movement_type
{
    public Guid movement_type_id { get; set; }

    public string name { get; set; } = null!;

    /// <summary>
    /// IN = entrada de stock. OUT = salida de stock.
    /// </summary>
    public string direction { get; set; } = null!;

    public bool is_active { get; set; }

    public virtual ICollection<inventory_movement> inventory_movements { get; set; } = new List<inventory_movement>();
}
