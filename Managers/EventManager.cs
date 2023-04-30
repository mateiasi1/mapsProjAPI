using mapsProjAPI.Data;
using mapsProjAPI.DTOs.Response.Events;
using mapsProjAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace mapsProjAPI.Managers
{
    public class EventManager : IEventManager
    {
        private readonly IConfiguration _configuration;
        private readonly Context _context;

        public EventManager(IConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<List<EventDTO>> GetEventsAsync()
        {
            var events = await _context.Events.Include(ev => ev.SubEvents).ThenInclude(sub => sub.SubEventTypes).ToListAsync();
            var eventDtos = events.Select(ev => new EventDTO
            {
                Id = ev.Id,
                Title = ev.Title,
                Icon = ev.Icon,
                Color = ev.Color,
                SubEvents = ev.SubEvents.Select(sub => new SubEventDto
                {
                    Id = sub.Id,
                    Title = sub.Title,
                    Icon = sub.Icon,
                    Color = sub.Color,
                    SubEventTypes = sub.SubEventTypes.Select(type => new SubEventTypeDto
                    {
                        Id = type.Id,
                        Title = type.Title,
                        Icon = type.Icon,
                        Color = type.Color
                    }).ToList()
                }).ToList()
            }).ToList();

            return eventDtos;
        }
    }
}
