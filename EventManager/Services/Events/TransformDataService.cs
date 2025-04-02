using EventManager.Entities;
using EventManager.Models.Event;
using EventManager.Models.User;

namespace EventManager.Services.Events
{
    public class TransformDataService : ITransformDataService
    {
        public EventDto EventToDto(Event @event)
        {
            var CreatedByDto = new UserDto
            {
                Id = @event.CreatedBy!.Id,
                Name = @event.CreatedBy.Name,
                Username = @event.CreatedBy.Username,
                Email = @event.CreatedBy.Email
            };

            var ParticipantsDto = new List<UserDto>();

            if (@event.Participants != null)
            {
                foreach (User Participant in @event.Participants)
                {
                    ParticipantsDto.Add(
                        new UserDto
                        {
                            Id = Participant.Id,
                            Name = Participant.Name,
                            Username = Participant.Username,
                            Email = Participant.Email
                        }
                    );
                }
            }

            return new EventDto
            {
                Id = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                Date = @event.Date,
                Location = @event.Location,
                CreatedDate = @event.CreatedDate,
                LastUpdatedDate = @event.LastUpdatedDate,
                CreatedBy = CreatedByDto,
                Participants = ParticipantsDto
            };
        }
    }
}
