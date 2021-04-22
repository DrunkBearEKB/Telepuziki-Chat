using System;
using System.Collections.Generic;
using System.Linq;
using Network.Extensions;

namespace Network.Package.ExchangingPackages
{
    public class FilePackage : IPackage
    {
        public PackageType Type => PackageType.File;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public byte[] Content { get; }
        public byte[] RawData { get; }

        public FilePackage(string idReceiver, string idAuthor, DateTime time, byte[] content)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = content;
            
            this.RawData = PackageCreator.GetRawFormattedData(
                    this.Type,
                    new []
                    {
                        this.IdReceiver, 
                        this.IdAuthor, 
                        PackageCreator.GetFormattedTime(this.Time)
                    },
                    this.Content)
                .ToArray();
        }
        
        public FilePackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);
            
            // TODO Создание конструкторов для FilePackage
            
            this.RawData = bytes;
        }
    }
}