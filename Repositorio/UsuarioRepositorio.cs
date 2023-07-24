using Tienda.Repositorio.IRepositorio;
using Tienda.Modelo;
using Tienda.DBContext;

namespace Tienda.Repositorio
{
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private readonly SistemaVentasContext _dbContext;

        public UsuarioRepositorio(SistemaVentasContext dbContext) : base(dbContext){
            _dbContext = dbContext;
        }

        public async Task<Usuario> Actualizar(Usuario entidad){
            _dbContext.Update(entidad);
            await _dbContext.SaveChangesAsync();
            return entidad;
        }
    }
}
