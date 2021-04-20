using System;
using System.Collections.Generic;
using System.Linq;

using Network.Extensions;

namespace Network.Package.ExchangingPackages
{
    public class VoicePackage : IPackage
    {
        public PackageType Type => PackageType.Voice;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public byte[] Content { get; }
        public byte[] RawData { get; }

        public VoicePackage(string idReceiver, string idAuthor, DateTime time, byte[] content)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = content;

            this.RawData = PackageCreator.GetRawFormattedData(
                    this.Type,
                    new []{ this.IdReceiver, this.IdAuthor, this.Time.ToString() },
                    this.Content)
                .ToArray();
        }

        public VoicePackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);
            
            this.IdReceiver = PackageCreator.Encoding.GetString(listParsed[0]);
            this.IdAuthor = PackageCreator.Encoding.GetString(listParsed[1]);
            this.Time = DateTime.Parse(PackageCreator.Encoding.GetString(listParsed[2]));
            this.Content = listParsed[3];
            
            this.RawData = bytes;
        }
    }
}