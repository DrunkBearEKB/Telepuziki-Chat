using System;
using System.Collections.Generic;
using System.Linq;

using Network.Extensions;

namespace Network.Package.ExchangingPackages
{
    public class HistoryRequestPackage : IPackage
    {
        public PackageType Type => PackageType.HistoryRequest;
        public string IdReceiver { get; }
        public string IdAuthor { get; }
        public string IdRequest { get; }
        public DateTime TimeUntil { get; }
        public byte[] RawData { get; }

        public HistoryRequestPackage(string idReceiver, string idAuthor, string idRequest, DateTime timeUntil)
        {
            this.IdReceiver = idReceiver;
            this.IdAuthor = idAuthor;
            this.IdRequest = idRequest;
            this.TimeUntil = timeUntil;
            
            this.RawData = PackageCreator.GetRawFormattedData(
                this.Type, 
                this.IdReceiver,
                this.IdAuthor,
                this.IdRequest,
                PackageCreator.GetFormattedTime(timeUntil));
        }
        
        public HistoryRequestPackage(byte[] bytes)
        {
            List<byte[]> listParsed = PackageCreator.Parse(bytes);
            
            this.IdReceiver = PackageCreator.Encoding.GetString(listParsed[0]);
            this.IdAuthor = PackageCreator.Encoding.GetString(listParsed[1]);
            this.IdRequest = PackageCreator.Encoding.GetString(listParsed[2]);
            this.TimeUntil = PackageCreator.ParseTime(listParsed[3]);

            this.RawData = bytes;
        }
    }
}