using System.Collections.Generic;
using EWork.Models;
using EWork.Models.Json;

namespace EWork.Services.Mappers.Interfaces
{
    public interface IMessageMapper
    {
        JsonMessage Map(Message message);
        IEnumerable<JsonMessage> MapRange(IEnumerable<Message> messages);
    }
}
