using DAL.Models;
using DAL.Repositories;

namespace Services;

public class EventService(IBaseRepository<Event, Guid> eventRepository) : IEventService
{
    public async Task<List<ValuesAtMinute>> GetValuesAtMinutes(DateTime? from, DateTime? to)
    {
        IQueryable<Event> events = eventRepository.GetAllQuery();

        if (!events.Any())
        {
            return [];
        }

        if (from is not null && to is null)
        {
            events = events.Where(e => e.CreateDateTime >= from);
        }

        if (from is null && to is not null)
        {
            events = events.Where(e => e.CreateDateTime <= to);
        }
        if (from is not null && to is not null)
        {
            events = events.Where(e => e.CreateDateTime >= from && e.CreateDateTime <= to);
        }

        return GetValuesByMinute(events);
    }

    public async Task<Event> Create(Event @event)
    {
        return await eventRepository.Create(@event);
    }

    private static List<ValuesAtMinute> GetValuesByMinute(IQueryable<Event> events)
    {
        TimeSpan interval = new(0, 1, 0);
        List<Event> eventsList = events.ToList();

        return eventsList
            .GroupBy(e => e.CreateDateTime.Ticks / interval.Ticks)
            .Select(s => new ValuesAtMinute()
            {
                ParticularMinute = new DateTime(s.Key * interval.Ticks),
                TotalValue = s.Sum(x => x.Value)
            }).ToList();
    }
}