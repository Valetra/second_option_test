using Microsoft.AspNetCore.Mvc;
using AutoMapper;

using Services;

namespace Controllers;

[ApiController]
public class EventController(IEventService eventService, IMapper mapper) : ControllerBase
{
    [HttpGet("Events")]
    public async Task<ActionResult<List<ResponseObjects.ValuesAtMinute>>> GetEventsInRange([FromQuery] RequestObjects.GetEventsInRangeParameters parameters)
    {

    }

    [HttpPost("Event")]
    public async Task<ActionResult> CreateEvent(RequestObjects.Event @event)
    {
        await eventService.Create(mapper.Map<DAL.Models.Event>(@event));

        return Ok();
    }
}