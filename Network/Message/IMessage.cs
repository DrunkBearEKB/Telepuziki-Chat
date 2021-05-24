using System;

namespace Network.Message
{
    public interface IMessage
    {
        MessageType Type { get; }
        // string Text { get; }
        // string ChatId { get; }
        string IdReceiver { get; }
        string IdAuthor { get; }
        DateTime Time { get; }
    }
}