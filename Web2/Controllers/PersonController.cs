using Bussines.interfaces;
using Entity.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Web2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IpersonBusiness _personBusiness;

        public PersonController(IpersonBusiness personBusiness)
        {
            _personBusiness = personBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var persons = await _personBusiness.GetAllAsync();
            return Ok(persons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _personBusiness.GetByIdAsync(id);
            if (person == null)
                return NotFound();
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonDto personDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _personBusiness.CreateAsync(personDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonDto personDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _personBusiness.UpdateAsync(id, personDto);
            if (updated==null)
                return NoContent();

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _personBusiness.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
