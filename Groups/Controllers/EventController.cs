using DAL.Dtos;
using DAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace Groups.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EventController : Controller
    {
        private readonly IEvent _dbEvent;
        public EventController(IEvent dbEvent)
        {
            _dbEvent = dbEvent;
        }

        [HttpPost("{groupId}")]
        public async Task<IActionResult> Post([FromBody] EventDto _event,int groupId)
        {
            bool create = await _dbEvent.createEvent(_event, groupId);
            if (create)
                return Ok();
            return BadRequest();
        }

        [HttpGet("{eventId}")]
        public async Task<Event> Get(int eventId)
        {
            var @event = await _dbEvent.getEventById(eventId);
            return @event;
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> Delete(int eventId)
        {
            bool ans =await _dbEvent.deleteEvent(eventId);
            if (ans)
                return Ok();
            return BadRequest();
        }

    }
}