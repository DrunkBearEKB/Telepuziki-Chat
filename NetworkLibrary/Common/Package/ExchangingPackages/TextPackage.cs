using System;
using System.Linq;

namespace NetworkLibrary.Common.Package.ExchangingPackages
{
    public class TextPackage : IPackage
    {
        public PackageType Type => PackageType.Text;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public string Content { get; }
        public byte[] Data { get; }

        public TextPackage(string idReceiver, string idAuthor, DateTime time, string content)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = content;

            this.Data = new byte[] { (byte)this.Type }
                .Concat(BitConverter.GetBytes(2 * PackageCreator.AmountBytesId + PackageCreator.AmountBytesTime + 
                                              this.Content.Length))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdReceiver, PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdAuthor, PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.Time.ToString(), PackageCreator.AmountBytesTime))
                .Concat(PackageCreator.Encoding.GetBytes(this.Content))
                .ToArray();
        }

        public TextPackage(byte[] bytes)
        {
            this.IdReceiver = PackageCreator.Encoding.GetString(
                bytes.Sub(5, PackageCreator.AmountBytesId));
            this.IdAuthor = PackageCreator.Encoding.GetString(
                bytes.Sub(5 + PackageCreator.AmountBytesId, PackageCreator.AmountBytesId));
            this.Time = DateTime.Parse(PackageCreator.Encoding.GetString(
                bytes.Sub(5 + 2 * PackageCreator.AmountBytesId, PackageCreator.AmountBytesTime)));
            this.Content = PackageCreator.Encoding.GetString(
                bytes.Sub(5 + 2 * PackageCreator.AmountBytesId + PackageCreator.AmountBytesTime));
            
            this.Data = bytes;
        }
    }
}