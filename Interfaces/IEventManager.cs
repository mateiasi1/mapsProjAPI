using mapsProjAPI.DTOs.Response.Events;

namespace mapsProjAPI.Interfaces
{
    public interface IEventManager
    {
        public Task<List<EventDTO>> GetEventsAsync();
    }
}
