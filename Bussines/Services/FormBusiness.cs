using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    public class FormBusiness : ServiceBase<FormDto, Form>, IFormBusiness
    {
        public FormBusiness(IFormRepository repository, ILogger<FormBusiness> logger)
            : base(repository, logger)
        {
        }

        public async Task<FormDto> Update(int id, string name, string? description, string? status)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Form with ID {id} not found.");
            }

            entity.Name = name;
            entity.Description = description;
            entity.Status = status;

            await _repository.UpdateAsync(entity);

            // Devuelve el DTO actualizado
            return new FormDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                DateCreation = entity.DateCreation,
                IsActive = entity.IsActive,
                Status = entity.Status
            };
        }

    }
}
