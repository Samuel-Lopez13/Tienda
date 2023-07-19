using System;
using System.Collections.Generic;

namespace Tienda;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public double? Precio { get; set; }

    public int? Stock { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
