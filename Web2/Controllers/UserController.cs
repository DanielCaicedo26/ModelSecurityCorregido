using Bussines;
using Bussines.interfaces;
using Bussines.Services;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserBusiness userBusiness, ILogger<UserController> logger)
        {
            _userBusiness = userBusiness;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userBusiness.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userBusiness.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado." });
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto userDto)
        {
            var created = await _userBusiness.CreateAsync(userDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.Id)
                return BadRequest(new { message = "El ID en la URL no coincide con el ID en el cuerpo." });

            var updated = await _userBusiness.Update(id,userDto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userBusiness.DeleteAsync(id);
            return Ok(new { message = "Usuario eliminado exitosamente." });
        }

        [HttpPatch("{id}/active")]
        public async Task<IActionResult> SetActiveStatus(int id, [FromBody] bool isActive)
        {
            var updated = await _userBusiness.SetActiveStatusAsync(id, isActive);
            return Ok(updated);
        }

       

    }
}
