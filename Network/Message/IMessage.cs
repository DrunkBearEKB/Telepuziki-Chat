using System;

namespace Network.Message
{
    public interface IMessage
    {
        MessageType Type { get; }
        string IdAuthor { get; }
        DateTime Time { get; }
    }
}