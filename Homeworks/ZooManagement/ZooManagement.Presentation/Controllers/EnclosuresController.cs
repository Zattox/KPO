using Microsoft.AspNetCore.Mvc;
using System;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.DTOs;
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

                // Создаем новый DTO с установленным Id
                var createdDto = new EnclosureDto
                {
                    Id = enclosure.Id,
                    Type = dto.Type,
                    Size = dto.Size,
                    CurrentAnimalCount = 0,
                    MaxCapacity = dto.MaxCapacity
                };
                return CreatedAtAction(nameof(GetAll), new { id = enclosure.Id }, createdDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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