namespace mapsProjAPI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public List<SubEvent> SubEvents { get; set; }
    }

}
