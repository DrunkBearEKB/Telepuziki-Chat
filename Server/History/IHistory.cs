using System.Collections.Generic;

using Network.Message;

namespace Server.History
{
    public interface IHistory
    {
        List<IMessage> GetClientHistoryAll(string idClient1, string idClient2);
        
        List<IMessage> GetClientHistoryTextMessages(string idClient1, string idClient2);
        
        List<IMessage> GetClientHistoryVoiceMessages(string idClient1, string idClient2);
        
        List<IMessage> GetClientHistoryFileMessages(string idClient1, string idClient2);
    }
}