using System.ComponentModel.DataAnnotations.Schema;

namespace EventManager.Models.Event
{
    public class EventCreateDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime Date { get; set; }
        public required string Location { get; set; }
    }
}
