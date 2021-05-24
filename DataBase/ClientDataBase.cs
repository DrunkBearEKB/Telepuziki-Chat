using System.Collections.Generic;
using Network.Message;

namespace DataBase
{
    public class ClientDataBase
    {
        private WrappedFirebase db { get; }
        public ClientDataBase()
        {
            db = new WrappedFirebase();
        }
        public List<IMessage> GetMessages(string id) => db.GetChat(id).Messages;
    }
}