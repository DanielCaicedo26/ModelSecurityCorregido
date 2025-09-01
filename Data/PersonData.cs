using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web2.Services;

namespace Data
{
    public class PersonData
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ILogger<PersonData> _logger;

        public PersonData(IDbContextProvider dbContextProvider, ILogger<PersonData> logger)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        private ApplicationDbContext _context => _dbContextProvider.GetDbContext();

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            try
            {
                var persons = await _context.Set<Person>()
                    .AsNoTracking()
                    .ToListAsync();

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

        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                var person = await _context.Set<Person>()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (person != null)
                {
                    if (person.DocumentNumber == null)
                    {
                        person.DocumentNumber = $"SIN-DOCUMENTO-{person.Id}";
                    }
                    if (person.DocumentType == null)
                    {
                        person.DocumentType = "NO ESPECIFICADO";
                    }

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

        public async Task<IEnumerable<Person>> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                var persons = await _context.Set<Person>()
                    .Where(p => p.DocumentNumber == documentNumber)
                    .AsNoTracking()
                    .ToListAsync();

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

        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
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