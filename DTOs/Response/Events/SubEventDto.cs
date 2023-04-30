using mapsProjAPI.Models;

namespace mapsProjAPI.DTOs.Response.Events
{
    public class SubEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public List<SubEventTypeDto> SubEventTypes { get; set; }
    }
}
