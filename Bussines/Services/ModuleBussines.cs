using Bussines.Core;
using Bussines.interfaces;
using Data.Interfaces;
using Data.Repositories;
using Entity.Dto;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Bussines.Services
{
    public class ModuleBusiness : ServiceBase<ModuleDto, Module>, IModuleBusiness
    {
        public ModuleBusiness(IModuleRepository repository, ILogger<ModuleBusiness> logger)
            : base(repository, logger)
        {
        }

        public async Task<ModuleDto> Update(int id, string name, string? description, string? status)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Module with ID {id} not found.");
            }

            entity.Name = name;
            entity.Description = description;
            entity.Statu = status;

            await _repository.UpdateAsync(entity);

            // Devuelve el DTO actualizado
            return new ModuleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Statu = entity.Statu,
                IsActive = entity.IsActive
            };
        }
    }
}