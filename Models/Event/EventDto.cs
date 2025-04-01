using EventManager.Models.User;

namespace EventManager.Models.Event
{
    public class EventDto
    {

        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime Date { get; set; }
        public required string Location { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public UserDto? CreatedBy { get; set; }
        public List<UserDto>? Participants { get; set; }
    }
}
