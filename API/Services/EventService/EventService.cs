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
            return new List<ValuesAtMinute>();
        }

        if (from is not null && to is null)
        {
            IQueryable<Event> eventsFrom = events.Where(e => e.CreateDateTime >= from);

            return GetValuesByMinute(eventsFrom);
        }

        if (from is null && to is not null)
        {
            IQueryable<Event> eventsTo = events.Where(e => e.CreateDateTime <= to);

            return GetValuesByMinute(eventsTo);
        }

        if (from is not null && to is not null)
        {
            IQueryable<Event> eventsInRange = events.Where(e => e.CreateDateTime >= from && e.CreateDateTime <= to);

            return GetValuesByMinute(eventsInRange);
        }

        return GetValuesByMinute(events);
    }

    public async Task<Event> Create(Event @event)
    {
        return await eventRepository.Create(@event);
    }

    private List<ValuesAtMinute> GetValuesByMinute(IQueryable<Event> events)
    {
        Event? firstEvent = events.FirstOrDefault();

        if (firstEvent is null)
        {
            return new List<ValuesAtMinute>();
        }

        DateTime minuteStart = new DateTime();
        List<ValuesAtMinute> result = new List<ValuesAtMinute>();
        ValuesAtMinute valuesAtMinute = new ValuesAtMinute();

        valuesAtMinute.ParticularMinute = new DateTime(firstEvent.CreateDateTime.Year, firstEvent.CreateDateTime.Month, firstEvent.CreateDateTime.Day, firstEvent.CreateDateTime.Hour, firstEvent.CreateDateTime.Minute, 0);

        foreach (var @event in events)
        {
            DateTime eventMinuteStart = new DateTime(@event.CreateDateTime.Year, @event.CreateDateTime.Month, @event.CreateDateTime.Day, @event.CreateDateTime.Hour, @event.CreateDateTime.Minute, 0);

            if (valuesAtMinute.ParticularMinute != eventMinuteStart)
            {
                result.Add(new ValuesAtMinute()
                {
                    ParticularMinute = valuesAtMinute.ParticularMinute,
                    Values = valuesAtMinute.Values
                });

                valuesAtMinute.ParticularMinute = new DateTime();
                valuesAtMinute.Values = 0;
            }

            if (eventMinuteStart != minuteStart)
            {
                minuteStart = new DateTime(@event.CreateDateTime.Year, @event.CreateDateTime.Month, @event.CreateDateTime.Day, @event.CreateDateTime.Hour, @event.CreateDateTime.Minute, 0);

                valuesAtMinute.ParticularMinute = minuteStart;
                valuesAtMinute.Values = @event.Value;
            }
            else
            {
                valuesAtMinute.Values += @event.Value;
            }
        }

        result.Add(new ValuesAtMinute()
        {
            ParticularMinute = valuesAtMinute.ParticularMinute,
            Values = valuesAtMinute.Values
        });

        return result;
    }
}