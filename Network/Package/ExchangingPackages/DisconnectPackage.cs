using System.Collections.Generic;
using System.Linq;

using Network.Extensions;

namespace Network.Package.ExchangingPackages
{
    public class DisconnectPackage : IPackage
    {
        public PackageType Type => PackageType.Disconnect;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public byte[] RawData { get; }

        public DisconnectPackage(string idReceiver, string idAuthor)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;

            this.RawData = PackageCreator.GetRawFormattedData(
                    this.Type,
                    this.IdReceiver, 
                    this.IdAuthor);
        }
        
        public DisconnectPackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);
            
            this.IdReceiver = PackageCreator.Encoding.GetString(listParsed[0]);
            this.IdAuthor = PackageCreator.Encoding.GetString(listParsed[1]);

            this.RawData = bytes;
        }
    }
}