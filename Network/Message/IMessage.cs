using System;

namespace Network.Message
{
    public interface IMessage
    {
        MessageType Type { get; }
        string IdReceiver { get; }
        string IdAuthor { get; }
        DateTime Time { get; }
    }
}