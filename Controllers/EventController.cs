using mapsProjAPI.Interfaces;
using mapsProjAPI.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mapsProjAPI.Controllers
{
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventManager _eventManager;
        public EventController(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/get-events")]
        public IActionResult GetEvents()
        {
            var events = _eventManager.GetEventsAsync();
            if(events == null)
            {
                return NotFound();
            }
            return Ok(events);
           
        }
    }
}
