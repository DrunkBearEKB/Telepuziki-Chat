using System;
using System.IO;

namespace Network.Message.ExchangingMessages
{
    public class FileMessage : IMessage
    {
        public MessageType Type => MessageType.Text;
        public string IdAuthor { get; }
        public DateTime Time { get; }
        
        public FileStream File { get; }
        
        public FileMessage(string idAuthor, DateTime time, byte[] bytes)
        {
            this.IdAuthor = idAuthor;
            this.Time = time;
            
            // TODO Создание конструктора для FileMessage
            // this.File = new FileStream(bytes);
        }
    }
}