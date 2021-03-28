namespace NetworkLibrary.Common.Package.ExchangingPackages
{
    public class HistoryRequestPackage : IPackage
    {
        public PackageType Type => PackageType.HistoryRequest;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public byte[] Data { get; }

        public HistoryRequestPackage(string idReceiver, string idAuthor)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
        }
        
        public HistoryRequestPackage(byte[] bytes)
        {
            this.IdReceiver = PackageCreator.Encoding.GetString(
                bytes.Sub(5, PackageCreator.AmountBytesId));
            this.IdAuthor = PackageCreator.Encoding.GetString(
                bytes.Sub(5 + PackageCreator.AmountBytesId, PackageCreator.AmountBytesId));

            this.Data = bytes;
        }
    }
}