using System;
using System.Linq;

namespace NetworkLibrary.Common.Package.ExchangingPackages
{
    public class VoicePackage : IPackage
    {
        public PackageType Type => PackageType.Voice;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public DateTime Time { get; }
        public byte[] Content { get; }
        public byte[] Data { get; }

        public VoicePackage(string idReceiver, string idAuthor, DateTime time, byte[] content)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Time = time;
            this.Content = content;

            this.Data = new byte[] { (byte)this.Type }
                .Concat(BitConverter.GetBytes(2 * PackageCreator.AmountBytesId + this.Content.Length))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdReceiver, PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.IdAuthor, PackageCreator.AmountBytesId))
                .Concat(PackageCreator.Encoding.GetBytes(this.Time.ToString(), PackageCreator.AmountBytesTime))
                .Concat(this.Content)
                .ToArray();
        }

        public VoicePackage(byte[] bytes)
        {
            this.IdReceiver = PackageCreator.Encoding.GetString(
                bytes.Sub(5, PackageCreator.AmountBytesId));
            this.IdAuthor = PackageCreator.Encoding.GetString(
                bytes.Sub(5 + PackageCreator.AmountBytesId, PackageCreator.AmountBytesId));
            this.Time = DateTime.Parse(PackageCreator.Encoding.GetString(
                bytes.Sub(5 + 2 * PackageCreator.AmountBytesId, PackageCreator.AmountBytesTime)));
            this.Content = bytes.Sub(5 + 2 * PackageCreator.AmountBytesId + PackageCreator.AmountBytesTime);
            
            this.Data = bytes;
        }
    }
}