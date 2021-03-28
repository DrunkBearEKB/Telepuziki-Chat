using System;

namespace NetworkLibrary.Common.Package.ExchangingPackages
{
    public class FilePackage : IPackage
    {
        public PackageType Type => PackageType.File;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public byte[] Content { get; }
        public byte[] Data { get; }

        public FilePackage(byte[] bytes)
        {
            // TODO Создание конструкторов для FilePackage
            
            this.Data = bytes;
        }
    }
}