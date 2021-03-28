using System;

namespace NetworkLibrary.Common.Message
{
    public interface IMessage
    {
        MessageType Type { get; }
        string IdAuthor { get; }
        DateTime Time { get; }
    }
}