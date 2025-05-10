using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Units.Dtos;
using ICookThis.Modules.Units.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Units.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService _service;
        public UnitsController(IUnitService service) => _service = service;

        [HttpGet]
        public Task<IEnumerable<UnitResponse>> GetAll() => _service.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitResponse>> Get(int id)
        {
            var u = await _service.GetByIdAsync(id);
            if (u == null) return NotFound();
            return Ok(u);
        }

        [HttpPost]
        public async Task<ActionResult<UnitResponse>> Create([FromBody] NewUnitRequest dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UnitResponse>> Update(int id, [FromBody] UpdateUnitRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public Task Delete(int id) => _service.DeleteAsync(id);
    }
}