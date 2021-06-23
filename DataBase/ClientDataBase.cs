using System.Collections.Generic;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace DataBase
{
    public class ClientDataBase
    {
        private WrappedFirebase db { get; }
        public ClientDataBase()
        {
            db = new WrappedFirebase();
        }
        public List<TextMessage> GetMessages(string id) => db.GetChat(id).Messages;
    }
}