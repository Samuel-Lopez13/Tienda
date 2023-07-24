using Tienda.Modelo;

namespace Tienda.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>{
        Task<Usuario> Actualizar(Usuario entidad);
    }
}
