using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tienda;

public partial class Usuario
{
    public int UsuarioId { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Contrasena { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
