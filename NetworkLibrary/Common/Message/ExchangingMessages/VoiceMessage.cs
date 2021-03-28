using System;
using System.Media;

namespace NetworkLibrary.Common.Message.ExchangingMessages
{
    public class VoiceMessage : IMessage
    {
        public MessageType Type => MessageType.Text;
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public byte[] Content { get; }

        public VoiceMessage(string idAuthor, DateTime time, byte[] bytes)
        {
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = bytes;
        }
    }
}