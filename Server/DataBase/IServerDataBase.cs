using System;
using System.Collections.Generic;

using Network.Message;

namespace Server.DataBase
{
    public interface IServerDataBase
    {
        List<IMessage> GetMessages(string idClient1, string idClient2, DateTime timeUntil);
        void AddMessage(IMessage message);
        void RemoveMessage(IMessage message);
        List<string> GetUsersWithSimilarId(string id);
        void AddClient(string id);
        bool ContainsClient(string id);
    }
}