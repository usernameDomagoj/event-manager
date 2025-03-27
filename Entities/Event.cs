using System.ComponentModel.DataAnnotations.Schema;

namespace EventManager.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime Date { get; set; }
        public required string Location { get; set; }
        //public required string? CreatedBy { get; set; } // relation after UserController
        //public List<string>? Participants { get; set; } // relation: https://www.youtube.com/watch?v=kMewc-TjO2s
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }


        // relation one to many
        public int CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        // relation many to many
        public List<User>? Participants { get; set; }
    }
}
