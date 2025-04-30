using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Person en la base de datos.
    /// </summary>
    public class PersonData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{PersonData}"/> para el registro de logs.</param>
        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las personas almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de personas.</returns>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            try
            {
                // Obtenemos los datos sin incluir las propiedades que pueden causar problemas
                var persons = await _context.Set<Person>()
                    .AsNoTracking()
                    .ToListAsync();

                // Asignamos valores predeterminados a campos NULL
                foreach (var person in persons)
                {
                    if (person.DocumentNumber == null)
                    {
                        person.DocumentNumber = $"SIN-DOCUMENTO-{person.Id}";
                    }
                    if (person.DocumentType == null)
                    {
                        person.DocumentType = "NO ESPECIFICADO";
                    }
                }

                // Cargamos las propiedades de navegación manualmente si es necesario
                foreach (var person in persons)
                {
                    await _context.Entry(person)
                        .Reference(p => p.User)
                        .LoadAsync();

                    await _context.Entry(person)
                        .Collection(p => p.StateInfractions)
                        .LoadAsync();

                    await _context.Entry(person)
                        .Collection(p => p.PaymentUsers)
                        .LoadAsync();
                }

                return persons;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las personas");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una persona específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la persona.</param>
        /// <returns>La persona encontrada o null si no existe.</returns>
        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                // Primero obtenemos la persona sin incluir propiedades que puedan causar problemas
                var person = await _context.Set<Person>()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (person != null)
                {
                    // Asignamos valores predeterminados a campos NULL
                    if (person.DocumentNumber == null)
                    {
                        person.DocumentNumber = $"SIN-DOCUMENTO-{person.Id}";
                    }
                    if (person.DocumentType == null)
                    {
                        person.DocumentType = "NO ESPECIFICADO";
                    }

                    // Cargamos las propiedades de navegación manualmente
                    await _context.Entry(person)
                        .Reference(p => p.User)
                        .LoadAsync();

                    await _context.Entry(person)
                        .Collection(p => p.StateInfractions)
                        .LoadAsync();

                    await _context.Entry(person)
                        .Collection(p => p.PaymentUsers)
                        .LoadAsync();
                }

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID {PersonId}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene personas por su número de documento.
        /// </summary>
        /// <param name="documentNumber">Número de documento a buscar.</param>
        /// <returns>Lista de personas con el número de documento especificado.</returns>
        public async Task<IEnumerable<Person>> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                // Primero obtenemos las personas sin incluir propiedades que puedan causar problemas
                var persons = await _context.Set<Person>()
                    .Where(p => p.DocumentNumber == documentNumber)
                    .AsNoTracking()
                    .ToListAsync();

                // Asignamos valores predeterminados a campos NULL
                foreach (var person in persons)
                {
                    if (person.DocumentNumber == null)
                    {
                        person.DocumentNumber = $"SIN-DOCUMENTO-{person.Id}";
                    }
                    if (person.DocumentType == null)
                    {
                        person.DocumentType = "NO ESPECIFICADO";
                    }
                }

                // Cargamos las propiedades de navegación manualmente si es necesario
                foreach (var person in persons)
                {
                    await _context.Entry(person)
                        .Reference(p => p.User)
                        .LoadAsync();

                    await _context.Entry(person)
                        .Collection(p => p.StateInfractions)
                        .LoadAsync();

                    await _context.Entry(person)
                        .Collection(p => p.PaymentUsers)
                        .LoadAsync();
                }

                return persons;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personas con número de documento {DocumentNumber}", documentNumber);
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva persona en la base de datos.
        /// </summary>
        /// <param name="person">Instancia de la persona a crear.</param>
        /// <returns>La persona creada.</returns>
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                // Asegurarse de que los campos no sean NULL antes de guardar
                if (person.DocumentNumber == null)
                {
                    person.DocumentNumber = $"SIN-DOCUMENTO-{DateTime.Now.Ticks}";
                }
                if (person.DocumentType == null)
                {
                    person.DocumentType = "NO ESPECIFICADO";
                }

                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la persona");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una persona existente en la base de datos.
        /// </summary>
        /// <param name="person">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {
                var existingPerson = await _context.Set<Person>().FindAsync(person.Id);
                if (existingPerson == null)
                {
                    _logger.LogWarning("No se encontró la persona con ID {PersonId} para actualizar", person.Id);
                    return false;
                }

                // Asegurarse de que los campos no sean NULL antes de guardar
                if (person.DocumentNumber == null)
                {
                    person.DocumentNumber = $"SIN-DOCUMENTO-{person.Id}";
                }
                if (person.DocumentType == null)
                {
                    person.DocumentType = "NO ESPECIFICADO";
                }

                _context.Entry(existingPerson).CurrentValues.SetValues(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona");
                return false;
            }
        }

        /// <summary>
        /// Elimina una persona de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la persona a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una persona con ID inválido: {PersonId}", id);
                return false;
            }

            try
            {
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ninguna persona con ID: {PersonId}", id);
                    return false;
                }

                _context.Set<Person>().Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la persona con ID {PersonId}", id);
                return false;
            }
        }
    }
}