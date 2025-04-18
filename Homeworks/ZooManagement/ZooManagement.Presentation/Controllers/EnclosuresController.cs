using Microsoft.AspNetCore.Mvc;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/enclosures")]
    public class EnclosuresController : ControllerBase
    {
        private readonly IEnclosureRepository _enclosureRepository;

        public EnclosuresController(IEnclosureRepository enclosureRepository)
        {
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<EnclosureDto>> GetAll()
        {
            var enclosures = _enclosureRepository.GetAll();
            var dtos = enclosures.Select(e => new EnclosureDto
            {
                Id = e.Id,
                AllowedAnimalSpecies = e.AllowedAnimalType,
                Size = e.Size,
                CurrentAnimalCount = e.CurrentAnimalCount,
                MaxCapacity = e.MaxCapacity
            });
            return Ok(dtos);
        }

        [HttpPost]
        public ActionResult Add([FromBody] EnclosureDto dto)
        {
            var enclosure = new Enclosure(
                dto.AllowedAnimalSpecies,
                dto.Size,
                dto.MaxCapacity
            );
            _enclosureRepository.Add(enclosure);
            return CreatedAtAction(nameof(GetAll), new { id = enclosure.Id }, dto);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var enclosure = _enclosureRepository.GetById(id);
            if (enclosure == null)
                return NotFound();
            _enclosureRepository.Remove(id);
            return NoContent();
        }
    }
}