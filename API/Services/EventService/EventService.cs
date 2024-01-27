using DAL.Models;
using DAL.Repositories;

namespace Services;

public class EventService(IBaseRepository<Event, Guid> eventRepository) : IEventService
{
    public async Task<Event> Create(Event action)
    {
        return await eventRepository.Create(action);
    }
}