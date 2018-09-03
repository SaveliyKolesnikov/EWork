using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWork.Models.Json
{
    public class JsonMessage
    {
        public JsonMessage(JsonUser sender, JsonUser receiver, string text, DateTime sendDate)
        {
            Sender = sender;
            Receiver = receiver;
            Text = text;
            SendDate = sendDate;
        }

        public JsonUser Sender { get; }
        public JsonUser Receiver{ get; }
        public string Text { get; }
        public DateTime SendDate { get; }
    }
}
