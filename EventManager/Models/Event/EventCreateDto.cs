using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManager.Models.Event
{
    public class EventCreateDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public required string Title { get; set; }
        [StringLength(500, MinimumLength = 1)]
        public required string Description { get; set; }
        public DateTime Date { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public required string Location { get; set; }
    }
}
