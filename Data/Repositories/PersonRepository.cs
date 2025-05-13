using Data.Core;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Data.Repositories
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(ApplicationDbContext context, ILogger<PersonRepository> logger)
            : base(context, logger)
        {
        }

        public override async Task<IEnumerable<Person>> GetAllAsync()
        {
            var persons = await _context.Person
                .Include(p => p.User)
                .Include(p => p.StateInfractions)
                .Include(p => p.PaymentUsers)
                .AsNoTracking()
                .ToListAsync();

            foreach (var person in persons)
            {
                person.DocumentNumber ??= $"SIN-DOCUMENTO-{person.Id}";
                person.DocumentType ??= "NO ESPECIFICADO";
            }

            return persons;
        }

        public override async Task<Person?> GetByIdAsync(int id)
        {
            var person = await _context.Person
                .Include(p => p.User)
                .Include(p => p.StateInfractions)
                .Include(p => p.PaymentUsers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person != null)
            {
                person.DocumentNumber ??= $"SIN-DOCUMENTO-{person.Id}";
                person.DocumentType ??= "NO ESPECIFICADO";
            }

            return person;
        }

        public async Task<IEnumerable<Person>> GetByUserIdAsync(int userId)
        {
            var persons = await _context.Person
                .Where(p => p.User.Id == userId)
                .Include(p => p.User)
                .Include(p => p.StateInfractions)
                .Include(p => p.PaymentUsers)
                .AsNoTracking()
                .ToListAsync();

            foreach (var person in persons)
            {
                person.DocumentNumber ??= $"SIN-DOCUMENTO-{person.Id}";
                person.DocumentType ??= "NO ESPECIFICADO";
            }

            return persons;
        }

        public async Task<IEnumerable<Person>> GetByDocumentNumberAsync(string documentNumber)
        {
            var persons = await _context.Person
                .Where(p => p.DocumentNumber == documentNumber)
                .Include(p => p.User)
                .Include(p => p.StateInfractions)
                .Include(p => p.PaymentUsers)
                .AsNoTracking()
                .ToListAsync();

            foreach (var person in persons)
            {
                person.DocumentNumber ??= $"SIN-DOCUMENTO-{person.Id}";
                person.DocumentType ??= "NO ESPECIFICADO";
            }

            return persons;
        }

        public async Task<Person> CreateAsync(Person person)
        {
            person.DocumentNumber ??= $"SIN-DOCUMENTO-{DateTime.Now.Ticks}";
            person.DocumentType ??= "NO ESPECIFICADO";

            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public override async Task<bool> UpdateAsync(Person person)
        {
            var existingPerson = await _context.Person.FindAsync(person.Id);
            if (existingPerson == null)
                return false;

            person.DocumentNumber ??= $"SIN-DOCUMENTO-{person.Id}";
            person.DocumentType ??= "NO ESPECIFICADO";

            _context.Entry(existingPerson).CurrentValues.SetValues(person);
            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person == null)
                return false;

            _context.Person.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

    



