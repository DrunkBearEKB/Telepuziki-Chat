using System;
using System.Collections.Generic;
using System.Linq;

using Network.Extensions;
using Network.Message;

namespace Network.Package.ExchangingPackages
{
    public class HistoryAnswerPackage : IPackage
    {
        public PackageType Type => PackageType.HistoryRequest;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public List<Tuple<MessageType, string>> Messages { get; }
        public byte[] RawData { get; }
        
        private static char separatorMessages = '☭';
        private char separatorTypeContent = '!';

        public HistoryAnswerPackage(string idReceiver, string idAuthor, List<Tuple<MessageType, string>> messages)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Messages = messages;
            
            this.RawData = PackageCreator.GetRawFormattedData(
                    this.Type,
                    this.IdReceiver, 
                    this.IdAuthor,
                    string.Join(separatorMessages, this.Messages.Select(
                        pair => $"{pair.Item1}{separatorTypeContent}{pair.Item2}")));
        }
        
        public HistoryAnswerPackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);
            
            this.IdReceiver = PackageCreator.Encoding.GetString(listParsed[0]);
            this.IdAuthor = PackageCreator.Encoding.GetString(listParsed[1]);
            this.Messages = PackageCreator.Encoding.GetString(listParsed[2])
                .Split(separatorMessages)
                .Select(str =>
                {
                    string[] parsed = str.Split(separatorTypeContent);
                    return new Tuple<MessageType, string>((MessageType)int.Parse(parsed[0]), parsed[1]);
                }).ToList();

            this.RawData = bytes;
        }
    }
}