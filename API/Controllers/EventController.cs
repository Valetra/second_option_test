using Microsoft.AspNetCore.Mvc;
using AutoMapper;

using Services;

namespace Controllers;

[ApiController]
public class EventController(IEventService eventService, IMapper mapper) : ControllerBase
{
    [HttpPost("Event")]
    public async Task<ActionResult> CreateEvent(RequestObjects.Event action)
    {
        await eventService.Create(mapper.Map<DAL.Models.Event>(action));

        return Ok();
    }
}