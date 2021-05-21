using System.Collections.Generic;
using Network.Message;

namespace Client.DataBase
{
    public interface IClientDataBaseHandler
    {
        List<IMessage> GetAllMessages(string idClient1, string idClient2);
        
        List<IMessage> GetTextMessages(string idClient1, string idClient2);
        
        List<IMessage> GetVoiceMessages(string idClient1, string idClient2);
        
        List<IMessage> GetFileMs(string idClient1, string idClient2);
    }
}