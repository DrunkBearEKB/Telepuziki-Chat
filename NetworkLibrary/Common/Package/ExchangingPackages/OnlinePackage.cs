using System;
using System.Linq;

namespace NetworkLibrary.Common.Package.ExchangingPackages
{
    public class OnlinePackage : IPackage
    {
        public PackageType Type => PackageType.Online;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public byte[] Data { get; }

        public OnlinePackage(string idAuthor)
        {
            this.IdReceiver = "";
            this.IdAuthor = idAuthor;

            this.Data = new byte[] { (byte)this.Type }
                .Concat(BitConverter.GetBytes(2 * PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdReceiver, PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdAuthor, PackageCreator.AmountBytesId)) 
                .ToArray();
        }

        public OnlinePackage(byte[] bytes)
        {
            this.IdReceiver = "";
            this.IdAuthor = PackageCreator.Encoding.GetString(
                bytes.Sub(5 + PackageCreator.AmountBytesId, PackageCreator.AmountBytesId));

            this.Data = bytes;
        }
    }
}