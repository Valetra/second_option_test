using DAL.Models;

namespace Services;

public interface IEventService
{
    Task<List<ValuesAtMinute>> GetValuesAtMinutes(DateTime? from, DateTime? to);
    Task<Event> Create(Event @event);
}