using System.Collections.Generic;
using System.Linq;

namespace Network.Package.ExchangingPackages
{
    public class UsersListAnswerPackage : IPackage
    {
        public PackageType Type => PackageType.UsersListAnswer;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public List<string> Users { get; }
        public byte[] RawData { get; }

        private readonly char separator = '!';
        
        public UsersListAnswerPackage(string idReceiver, string idAuthor, List<string> users)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.Users = users;

            this.RawData = PackageCreator.GetRawFormattedData(
                this.Type,
                this.IdReceiver,
                this.IdAuthor,
                string.Join(separator, users));
        }

        public UsersListAnswerPackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);

            this.IdReceiver = PackageCreator.Encoding.GetString(listParsed[0]);
            this.IdAuthor = PackageCreator.Encoding.GetString(listParsed[1]);
            this.Users = PackageCreator.Encoding.GetString(listParsed[2]).Split(separator).ToList();
            
            this.RawData = bytes;
        }
    }
}