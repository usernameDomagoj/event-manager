using EventManager.Entities;
using EventManager.Models.Event;

namespace EventManager.Services.Events
{
    public interface ITransformDataService
    {
        EventDto EventToDto(Event @event);
    }
}
