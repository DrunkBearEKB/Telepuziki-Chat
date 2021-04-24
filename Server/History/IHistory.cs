using System.Collections.Generic;

using Network.Message;

namespace Server.History
{
    public interface IHistory
    {
        List<IMessage> GetAll(string idClient1, string idClient2);
        
        List<IMessage> GetTextMessages(string idClient1, string idClient2);
        
        List<IMessage> GetVoiceMessages(string idClient1, string idClient2);
        
        List<IMessage> GetFileMessages(string idClient1, string idClient2);
    }
}