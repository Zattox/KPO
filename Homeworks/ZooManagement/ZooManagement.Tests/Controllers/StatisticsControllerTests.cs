using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Services;
using ZooManagement.Infrastructure.Repositories;
using ZooManagement.Presentation.Controllers;

namespace ZooManagement.Tests.Controllers
{
    public class StatisticsControllerTests
    {
        private readonly StatisticsController _controller;
        private readonly ZooStatisticsService _statisticsService;
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;

        public StatisticsControllerTests()
        {
            _animalRepository = new InMemoryAnimalRepository();
            _enclosureRepository = new InMemoryEnclosureRepository();
            _statisticsService = new ZooStatisticsService(_animalRepository, _enclosureRepository);
            _controller = new StatisticsController(_statisticsService);
        }

        [Fact]
        public void GetStatistics_WhenDataExists_ShouldReturnOk()
        {
            // Act
            var result = _controller.GetStatistics();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<ZooStatisticsDto>(okResult.Value);
        }
    }
}