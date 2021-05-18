using System.Collections.Generic;
using Network.Message;

namespace Client.Common
{
    public class Contact
    {
        public string Id { get; }
        public bool HistoryReceived = false;

        public Contact(string id)
        {
            this.Id = id;
        }

        public List<IMessage> GetPreviousMessages()
        {
            return new List<IMessage>();
        }
    }
}