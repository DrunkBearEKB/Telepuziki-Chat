using System;
using System.Linq;

namespace NetworkLibrary.Common.Package.ExchangingPackages
{
    public class DisconnectPackage : IPackage
    {
        public PackageType Type => PackageType.Disconnect;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public byte[] Data { get; }

        public DisconnectPackage(string idAuthor)
        {
            this.IdReceiver = "";
            this.IdAuthor = idAuthor;
            
            this.Data = new byte[] { (byte)this.Type }
                .Concat(BitConverter.GetBytes(2 * PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdReceiver, PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdAuthor, PackageCreator.AmountBytesId))
                .ToArray();
        }
        
        public DisconnectPackage(byte[] bytes)
        {
            this.IdReceiver = PackageCreator.Encoding.GetString(
                bytes.Sub(5, PackageCreator.AmountBytesId));
            this.IdAuthor = PackageCreator.Encoding.GetString(
                bytes.Sub(5 + PackageCreator.AmountBytesId, PackageCreator.AmountBytesId));

            this.Data = bytes;
        }
    }
}