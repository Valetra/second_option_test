using DAL.Models;
using DAL.Repositories;

namespace Services;

public class EventService(IBaseRepository<Event, Guid> eventRepository) : IEventService
{
    public async Task<List<ValuesAtMinute>> GetValuesAtMinutes(DateTime from, DateTime? to)
    {
        IQueryable<Event> events = eventRepository.GetAllQuery();

        if (!events.Any())
        {
            return new List<ValuesAtMinute>();
        }

        if (to is not null)
        {
            IQueryable<Event> eventsInRange = events.Where(e => e.CreateDateTime >= from && e.CreateDateTime <= to);
            //TODO:Div range by minutes
            //TODO:Return List<ValuesAtMinute>
        }

        IQueryable<Event> eventsFrom = events.Where(e => e.CreateDateTime >= from);
        //TODO:Div range by minutes
        //TODO:Return List<ValuesAtMinute>
        return new List<ValuesAtMinute>();
    }

    public async Task<Event> Create(Event @event)
    {
        return await eventRepository.Create(@event);
    }
}