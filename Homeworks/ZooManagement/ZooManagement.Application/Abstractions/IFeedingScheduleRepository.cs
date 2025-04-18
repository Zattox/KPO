using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Application.Abstractions
{
    // Defines the contract for feeding schedule repository
    public interface IFeedingScheduleRepository
    {
        void Add(FeedingSchedule schedule);
        void Update(FeedingSchedule schedule);
        FeedingSchedule GetById(Guid scheduleId);
        IEnumerable<FeedingSchedule> GetAll();
    }
}