using System;
using Microsoft.AspNetCore.Mvc;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Services;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.ValueObjects;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/feedingschedules")]
    public class FeedingSchedulesController : ControllerBase
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly FeedingOrganizationService _feedingOrganizationService;

        public FeedingSchedulesController(
            IFeedingScheduleRepository feedingScheduleRepository,
            IAnimalRepository animalRepository,
            FeedingOrganizationService feedingOrganizationService)
        {
            _feedingScheduleRepository = feedingScheduleRepository ?? throw new ArgumentNullException(nameof(feedingScheduleRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _feedingOrganizationService = feedingOrganizationService ?? throw new ArgumentNullException(nameof(feedingOrganizationService));
        }

        [HttpGet]
        public ActionResult<IEnumerable<FeedingScheduleDto>> GetAll()
        {
            var schedules = _feedingScheduleRepository.GetAll();
            var dtos = schedules.Select(s => new FeedingScheduleDto
            {
                Id = s.Id,
                AnimalId = s.AnimalId,
                FeedingTime = s.FeedingTime.Value,
                FoodType = s.FoodType,
                IsCompleted = s.IsCompleted
            });
            return Ok(dtos);
        }

        [HttpPost]
        public ActionResult Add([FromBody] FeedingScheduleDto dto)
        {
            try
            {
                var animal = _animalRepository.GetById(dto.AnimalId);
                if (animal == null)
                    throw new ArgumentException("Animal not found.");
                if (animal.FavoriteFood != dto.FoodType)
                    throw new ArgumentException("Food type does not match animal's favorite food.");

                var schedule = new FeedingSchedule(
                    dto.AnimalId,
                    new FeedingTime(dto.FeedingTime),
                    dto.FoodType
                );
                _feedingScheduleRepository.Add(schedule);
                
                var createdDto = new FeedingScheduleDto
                {
                    Id = schedule.Id,
                    AnimalId = dto.AnimalId,
                    FeedingTime = dto.FeedingTime,
                    FoodType = dto.FoodType,
                    IsCompleted = false
                };
                return CreatedAtAction(nameof(GetAll), new { id = schedule.Id }, createdDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/complete")]
        public ActionResult CompleteFeeding(Guid id)
        {
            try
            {
                var @event = _feedingOrganizationService.CompleteFeeding(id);
                return Ok(@event);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public ActionResult Update(Guid id, [FromBody] FeedingScheduleDto dto)
        {
            try
            {
                _feedingOrganizationService.UpdateFeedingSchedule(id, new FeedingTime(dto.FeedingTime), dto.FoodType);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}