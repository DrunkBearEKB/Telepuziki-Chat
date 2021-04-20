using System;
using System.Collections.Generic;

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

        public FilePackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);
            
            // TODO Создание конструкторов для FilePackage
            
            this.RawData = bytes;
        }
    }
}