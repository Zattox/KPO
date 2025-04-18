using Microsoft.AspNetCore.Mvc;
using System;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.ValueObjects;

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

        // Retrieves all enclosures
        [HttpGet]
        public ActionResult<IEnumerable<EnclosureDto>> GetAll()
        {
            var enclosures = _enclosureRepository.GetAll();
            var dtos = enclosures.Select(e => new EnclosureDto
            {
                Id = e.Id,
                Type = e.Type,
                Size = e.Size.Value,
                CurrentAnimalCount = e.CurrentAnimalCount,
                MaxCapacity = e.MaxCapacity.Value
            });
            return Ok(dtos);
        }

        // Adds a new enclosure
        [HttpPost]
        public ActionResult Add([FromBody] EnclosureDto dto)
        {
            try
            {
                var enclosure = new Enclosure(
                    dto.Type,
                    new EnclosureSize(dto.Size),
                    new EnclosureCapacity(dto.MaxCapacity)
                );
                _enclosureRepository.Add(enclosure);
                return CreatedAtAction(nameof(GetAll), new { id = enclosure.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Deletes an enclosure
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