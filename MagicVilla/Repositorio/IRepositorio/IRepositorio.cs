using System.Linq.Expressions;

namespace MagicVilla.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null, string? incluirPropiedades = null);

        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true, string? incluirPropiedades = null);

        Task Crear(T entidad);

        Task Remover(T entidad);

        Task Grabar();
    }
}
