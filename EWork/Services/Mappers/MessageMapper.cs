using System.Collections.Generic;
using System.IO;
using System.Linq;
using EWork.Config;
using EWork.Models;
using EWork.Models.Json;
using EWork.Services.Mappers.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace EWork.Services.Mappers
{
    public class MessageMapper : IMessageMapper
    {
        private readonly IHostingEnvironment _environment;
        private readonly IOptions<PhotoConfig> _photoOptions;

        public MessageMapper(IHostingEnvironment environment, IOptions<PhotoConfig> photoOptions)
        {
            _environment = environment;
            _photoOptions = photoOptions;
        }

        public JsonMessage Map(Message message)
        {
            var pathToProfilePhotos = Path.Combine(_environment.ContentRootPath, _photoOptions.Value.UsersPhotosPath);

            var senderJson = new JsonUser(message.Sender.UserName,
                Path.Combine(pathToProfilePhotos, message.Sender.ProfilePhotoName));
            var receiverJson = new JsonUser(message.Receiver.UserName,
                Path.Combine(pathToProfilePhotos, message.Receiver.ProfilePhotoName));

            return new JsonMessage(receiver: receiverJson, sender: senderJson, text: message.Text, sendDate: message.SendDate);
        }

        public IEnumerable<JsonMessage> MapRange(IEnumerable<Message> messages) => messages.Select(Map);
    }
}
