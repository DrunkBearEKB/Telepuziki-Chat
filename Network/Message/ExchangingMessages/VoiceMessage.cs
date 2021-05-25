using System;

namespace Network.Message.ExchangingMessages
{
    public class VoiceMessage : IMessage
    {
        public MessageType Type => MessageType.Text;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public byte[] Content { get; }

        public VoiceMessage(string idReceiver, string idAuthor, DateTime time, byte[] bytes)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = bytes;
        }
    }
}