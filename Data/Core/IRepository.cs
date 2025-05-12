namespace Data.Core
{
    /// <summary>
    /// Define los métodos genéricos para acceder y manipular datos en el repositorio.
    /// </summary>
    /// <typeparam name="T">El tipo de entidad sobre el cual se aplican las operaciones.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene todos los registros de la entidad T.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro único por su identificador primario.
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Agrega una nueva entidad al contexto y la persiste en la base de datos.
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Actualiza una entidad existente en la base de datos.
        /// </summary>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// Elimina físicamente una entidad según su identificador.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Realiza una eliminación lógica, marcando IsActive como false.
        /// </summary>
        Task<bool> DeleteLogicalAsync(int id);
    }
}