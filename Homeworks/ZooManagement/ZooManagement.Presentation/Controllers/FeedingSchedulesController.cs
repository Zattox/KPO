using Microsoft.AspNetCore.Mvc;
using System;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Abstractions;
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
        private readonly FeedingOrganizationService _feedingOrganizationService;

        public FeedingSchedulesController(IFeedingScheduleRepository feedingScheduleRepository, FeedingOrganizationService feedingOrganizationService)
        {
            _feedingScheduleRepository = feedingScheduleRepository ?? throw new ArgumentNullException(nameof(feedingScheduleRepository));
            _feedingOrganizationService = feedingOrganizationService ?? throw new ArgumentNullException(nameof(feedingOrganizationService));
        }

        // Retrieves all feeding schedules
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

        // Adds a new feeding schedule
        [HttpPost]
        public ActionResult Add([FromBody] FeedingScheduleDto dto)
        {
            try
            {
                var schedule = new FeedingSchedule(
                    dto.AnimalId,
                    new FeedingTime(dto.FeedingTime),
                    dto.FoodType
                );
                _feedingScheduleRepository.Add(schedule);
                return CreatedAtAction(nameof(GetAll), new { id = schedule.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Marks a feeding as completed
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
    }
}