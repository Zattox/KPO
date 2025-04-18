using Microsoft.AspNetCore.Mvc;
using System;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.Services;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Domain.Enums;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly AnimalTransferService _animalTransferService;

        public AnimalsController(IAnimalRepository animalRepository, AnimalTransferService animalTransferService)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _animalTransferService = animalTransferService ?? throw new ArgumentNullException(nameof(animalTransferService));
        }

        // Retrieves all animals
        [HttpGet]
        public ActionResult<IEnumerable<AnimalDto>> GetAll()
        {
            var animals = _animalRepository.GetAll();
            var dtos = animals.Select(a => new AnimalDto
            {
                Id = a.Id,
                Species = a.Species,
                Name = a.Name.Value,
                DateOfBirth = a.DateOfBirth.Value,
                Gender = a.Gender,
                FavoriteFood = a.FavoriteFood,
                HealthStatus = a.HealthStatus,
                EnclosureId = a.EnclosureId
            });
            return Ok(dtos);
        }

        // Adds a new animal
        [HttpPost]
        public ActionResult Add([FromBody] AnimalDto dto)
        {
            try
            {
                var animal = new Animal(
                    dto.Species,
                    new AnimalName(dto.Name),
                    new BirthDate(dto.DateOfBirth),
                    dto.Gender,
                    dto.FavoriteFood
                );
                _animalRepository.Add(animal);
                return CreatedAtAction(nameof(GetAll), new { id = animal.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Deletes an animal
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var animal = _animalRepository.GetById(id);
            if (animal == null)
                return NotFound();
            _animalRepository.Remove(id);
            return NoContent();
        }

        // Transfers an animal to a new enclosure
        [HttpPost("{id}/transfer")]
        public ActionResult Transfer(Guid id, [FromBody] Guid enclosureId)
        {
            try
            {
                var @event = _animalTransferService.TransferAnimal(id, enclosureId);
                return Ok(@event);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}