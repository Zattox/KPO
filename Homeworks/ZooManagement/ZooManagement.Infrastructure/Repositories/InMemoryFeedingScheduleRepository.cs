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

        // Adds a new feeding schedule to the repository
        public void Add(FeedingSchedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));
            _schedules[schedule.Id] = schedule;
        }

        // Updates an existing feeding schedule in the repository
        public void Update(FeedingSchedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));
            if (!_schedules.ContainsKey(schedule.Id))
                throw new InvalidOperationException("Feeding schedule not found.");
            _schedules[schedule.Id] = schedule;
        }

        // Retrieves a feeding schedule by its ID
        public FeedingSchedule GetById(Guid scheduleId)
        {
            _schedules.TryGetValue(scheduleId, out var schedule);
            return schedule;
        }

        // Retrieves all feeding schedules
        public IEnumerable<FeedingSchedule> GetAll()
        {
            return _schedules.Values.ToList();
        }
    }
}