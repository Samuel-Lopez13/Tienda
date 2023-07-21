using System.ComponentModel.DataAnnotations;

namespace Tienda.Modelo.DTO{
    public class CrearUsuarioDTO{
        [Required]
        [MaxLength(30)]
        public string? Nombre { get; set; }
        [Required]
        public string? Apellido { get; set; }
        [Required]
        public string? Contrasena { get; set; }
    }
}
