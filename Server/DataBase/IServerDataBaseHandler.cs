using System.Collections.Generic;

using Network.Message;

namespace Server.DataBase
{
    public interface IServerDataBaseHandler
    {
        List<IMessage> GetAllMessages(string idClient1, string idClient2);
        
        List<IMessage> GetTextMessages(string idClient1, string idClient2);
        
        List<IMessage> GetVoiceMessages(string idClient1, string idClient2);
        
        List<IMessage> GetFiles(string idClient1, string idClient2);

        List<string> GetAllUsers();

        List<string> GetUsersWithSimilarId(string id);
    }
}