using Utilities.Exceptions;

namespace Bussines.Core
{
    /// <summary>
    /// Define el contrato base para los servicios de la capa de negocio.
    /// </summary>
    public interface IServiceBase<TDto, TEntity> where TEntity : class
    {
        /// <summary>
        /// Obtiene todos los registros en formato DTO.
        /// </summary>
        Task<IEnumerable<TDto>> GetAllAsync();

        /// <summary>
        /// Obtiene un solo registro por su identificador.
        /// </summary>
        Task<TDto> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo registro a partir de un DTO.
        /// </summary>
        Task<TDto> CreateAsync(TDto dto);

        /// <summary>
        /// Actualiza un registro existente a partir de un DTO.
        /// </summary>
        Task<TDto> Update(int id, TDto dto);

        /// <summary>
        /// Elimina permanentemente un registro del sistema.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Cambia el estado IsActive del registro.
        /// </summary>
        Task<TDto> SetActiveStatusAsync(int id, bool isActive);
    }
}