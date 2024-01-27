using DAL.Models;

namespace Services;

public interface IEventService
{
    Task<Event> Create(Event action);
}