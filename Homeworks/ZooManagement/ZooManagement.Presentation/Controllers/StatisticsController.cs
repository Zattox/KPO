using Microsoft.AspNetCore.Mvc;
using ZooManagement.Application.Services;
using ZooManagement.Application.DTOs;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly ZooStatisticsService _zooStatisticsService;

        public StatisticsController(ZooStatisticsService zooStatisticsService)
        {
            _zooStatisticsService = zooStatisticsService ?? throw new ArgumentNullException(nameof(zooStatisticsService));
        }

        // Retrieves zoo statistics
        [HttpGet]
        public ActionResult<ZooStatisticsDto> GetStatistics()
        {
            var stats = new ZooStatisticsDto
            {
                TotalAnimals = _zooStatisticsService.GetTotalAnimals(),
                FreeEnclosures = _zooStatisticsService.GetFreeEnclosures()
            };
            return Ok(stats);
        }
    }
}