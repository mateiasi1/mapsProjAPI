namespace mapsProjAPI.Models
{
    public class SubEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public List<SubEventType> SubEventTypes { get; set; }
    }
}
