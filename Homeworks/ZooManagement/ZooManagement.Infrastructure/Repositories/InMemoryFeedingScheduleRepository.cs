using System;
using System.Collections.Generic;
using System.Linq;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    // In-memory implementation of feeding schedule repository
    public class InMemoryFeedingScheduleRepository : IFeedingScheduleRepository
    {
        private readonly Dictionary<Guid, FeedingSchedule> _schedules = new Dictionary<Guid, FeedingSchedule>();

        public void Add(FeedingSchedule schedule)
        {
            _schedules[schedule.Id] = schedule;
        }

        public void Update(FeedingSchedule schedule)
        {
            _schedules[schedule.Id] = schedule;
        }

        public FeedingSchedule GetById(Guid scheduleId)
        {
            _schedules.TryGetValue(scheduleId, out var schedule);
            return schedule;
        }

        public IEnumerable<FeedingSchedule> GetAll()
        {
            return _schedules.Values.ToList();
        }
    }
}