using System.Linq.Expressions;

namespace Tienda.Repositorio.IRepositorio{

    //Creo repositorio generico, luego su implementacion
    //cree el de usuario y su implementacion para el update, luego agregarlo al program

    public interface IRepositorio<T> where T : class{

        Task Crear(T entidad);

        Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null);

        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true);

        Task Remover(T entidad);

        Task Grabar();
    }
}
