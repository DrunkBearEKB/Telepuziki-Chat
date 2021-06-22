using System.Collections.Generic;

namespace Network.Package.ExchangingPackages
{
    public class UsersListRequestPackage : IPackage
    {
        public PackageType Type => PackageType.UsersListRequest;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public string IdRequest { get; }
        public byte[] RawData { get; }
        
        public UsersListRequestPackage(string idReceiver, string idAuthor, string idRequest)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.IdRequest = idRequest;

            this.RawData = PackageCreator.GetRawFormattedData(
                this.Type,
                this.IdReceiver,
                this.IdAuthor,
                this.IdRequest);
        }

        public UsersListRequestPackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);

            this.IdReceiver = PackageCreator.Encoding.GetString(listParsed[0]);
            this.IdAuthor = PackageCreator.Encoding.GetString(listParsed[1]);
            this.IdRequest = PackageCreator.Encoding.GetString(listParsed[2]);
            
            this.RawData = bytes;
        }
    }
}