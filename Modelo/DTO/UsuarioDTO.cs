using System.ComponentModel.DataAnnotations;

namespace Tienda.Modelo.DTO{
    public class UsuarioDTO{
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string? Contrasena { get; set; }
    }
}
