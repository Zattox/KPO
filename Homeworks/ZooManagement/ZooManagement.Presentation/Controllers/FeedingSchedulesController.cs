using Microsoft.AspNetCore.Mvc;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.Services;
using ZooManagement.Domain.Entities;

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

        [HttpGet]
        public ActionResult<IEnumerable<FeedingScheduleDto>> GetAll()
        {
            var schedules = _feedingScheduleRepository.GetAll();
            var dtos = schedules.Select(s => new FeedingScheduleDto
            {
                Id = s.Id,
                AnimalId = s.AnimalId,
                FeedingTime = s.FeedingTime,
                FoodType = s.FoodType,
                IsCompleted = s.IsCompleted
            });
            return Ok(dtos);
        }

        [HttpPost]
        public ActionResult Add([FromBody] FeedingScheduleDto dto)
        {
            var schedule = new FeedingSchedule(
                dto.AnimalId,
                dto.FeedingTime,
                dto.FoodType
            );
            _feedingScheduleRepository.Add(schedule);
            return CreatedAtAction(nameof(GetAll), new { id = schedule.Id }, dto);
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
    }
}