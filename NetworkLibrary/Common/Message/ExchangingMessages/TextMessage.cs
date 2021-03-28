using System;

namespace NetworkLibrary.Common.Message.ExchangingMessages
{
    public class TextMessage : IMessage
    {
        public MessageType Type => MessageType.Text;
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public string Content { get; }

        public TextMessage(string idAuthor, DateTime time, string content)
        {
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = content;
        }
    }
}