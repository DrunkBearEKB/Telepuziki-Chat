using System;

namespace Network.Message.ExchangingMessages
{
    public class TextMessage : IMessage
    {
        public MessageType Type => MessageType.Text;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public string Content { get; }

        public TextMessage(string idReceiver, string idAuthor, DateTime time, string content)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = content;
        }
    }
}