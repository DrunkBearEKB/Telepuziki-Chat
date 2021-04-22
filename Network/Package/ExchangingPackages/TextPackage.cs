using System;
using System.Collections.Generic;

using System.Linq;

using Network.Extensions;

namespace Network.Package.ExchangingPackages
{
    public class TextPackage : IPackage
    {
        public PackageType Type => PackageType.Text;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public string Content { get; }
        public byte[] RawData { get; }

        public TextPackage(string idReceiver, string idAuthor, DateTime time, string content)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = content;

            this.RawData = PackageCreator.GetRawFormattedData(
                this.Type,
                this.IdReceiver,
                this.IdAuthor,
                PackageCreator.GetFormattedTime(this.Time),
                this.Content);
        }

        public TextPackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);

            this.IdReceiver = PackageCreator.Encoding.GetString(listParsed[0]);
            this.IdAuthor = PackageCreator.Encoding.GetString(listParsed[1]);
            this.Time = PackageCreator.ParseTime(listParsed[2]);
            this.Content = PackageCreator.Encoding.GetString(listParsed[3]);
            
            this.RawData = bytes;
        }
    }
}