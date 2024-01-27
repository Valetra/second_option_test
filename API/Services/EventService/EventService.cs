using DAL.Models;
using DAL.Repositories;

namespace Services;

public class EventService(IBaseRepository<Event, Guid> eventRepository) : IEventService
{
    public async Task<Event> Create(Event @event)
    {
        return await eventRepository.Create(@event);
    }
}