using System.Collections.Generic;
using System.Threading.Tasks;
using Network.Message;

namespace GDataBase
{
    public interface IDataBase
    {
        Task<List<string>> GetListClientsId();

        Task<List<IMessage>> GetMessages(string id1, string id2);

        void AddClient(string id);

        void AddMessage(IMessage message);
    }
}