using Microsoft.AspNetCore.Mvc;
using AutoMapper;

using Services;

namespace Controllers;

[ApiController]
public class EventController(IEventService eventService, IMapper mapper) : ControllerBase
{
    [HttpGet("Events")]
    public async Task<ActionResult<List<ResponseObjects.ValuesAtMinute>>> GetValuesAtMinutes([FromQuery] RequestObjects.GetEventsInRangeParameters parameters)
    {
        List<Services.ValuesAtMinute> valuesAtMinutes = await eventService.GetValuesAtMinutes(parameters.From, parameters.To);

        List<ResponseObjects.ValuesAtMinute> response = valuesAtMinutes.Select(v => mapper.Map<ResponseObjects.ValuesAtMinute>(v)).ToList();

        return Ok(response);
    }

    [HttpPost("Event")]
    public async Task<ActionResult> CreateEvent(RequestObjects.Event @event)
    {
        await eventService.Create(mapper.Map<DAL.Models.Event>(@event));

        return Ok();
    }
}