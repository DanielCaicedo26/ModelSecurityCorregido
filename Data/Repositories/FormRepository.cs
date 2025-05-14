// Ensure FormRepository implements IFormRepository
using Data.Interfaces;
using Entity.Context;
using Entity.Dto;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormRepository> _logger;

        public FormRepository(ApplicationDbContext context, ILogger<FormRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            return await _context.Form.ToListAsync();
        }

        public async Task<Form?> GetByIdAsync(int id)
        {
            return await _context.Form.FindAsync(id);
        }

        public async Task<Form> AddAsync(Form form)
        {
            _context.Form.Add(form);
            await _context.SaveChangesAsync();
            return form;
        }

        public async Task<bool> UpdateAsync(Form form)
        {
            _context.Form.Update(form);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var form = await _context.Form.FindAsync(id);
            if (form == null) return false;

            _context.Form.Remove(form);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLogicalAsync(int id)
        {
            var form = await _context.Form.FindAsync(id);
            if (form == null) return false;

            form.IsActive = false;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<FormDto> Update(int id, string name, string? description, string? status)
        {
            var form = await _context.Form.FindAsync(id);
            if (form == null) throw new KeyNotFoundException("Form not found");

            form.Name = name;
            form.Description = description;
            form.Status = status;
            await _context.SaveChangesAsync();

            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Description = form.Description,
                Status = form.Status
            };
        }
    }
}