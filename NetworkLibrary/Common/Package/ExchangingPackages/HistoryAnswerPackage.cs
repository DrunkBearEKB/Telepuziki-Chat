namespace NetworkLibrary.Common.Package.ExchangingPackages
{
    public class HistoryAnswerPackage : IPackage
    {
        public PackageType Type => PackageType.HistoryRequest;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public byte[] Data { get; }

        public HistoryAnswerPackage(byte[] bytes)
        {
            this.IdReceiver = PackageCreator.Encoding.GetString(
                bytes.Sub(5, PackageCreator.AmountBytesId));
            this.IdAuthor = PackageCreator.Encoding.GetString(
                bytes.Sub(5 + PackageCreator.AmountBytesId, PackageCreator.AmountBytesId));
            
            // TODO Создание конструкторов для HistoryAnswerPackage

            this.Data = bytes;
        }
    }
}