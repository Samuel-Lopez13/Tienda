namespace Tienda.Modelo { 

    public partial class Compra{
        public int CompraId { get; set; }

        public int UsuarioId { get; set; }

        public int ProductoId { get; set; }

        public virtual Producto Producto { get; set; } = null!;

        public virtual Usuario Usuario { get; set; } = null!;
    }

}