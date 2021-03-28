using System.Collections.Generic;

using NetworkLibrary.Common.Message;

namespace NetworkLibrary.Server
{
    public interface IHistory
    {
        List<IMessage> GetClientHistoryAll(string idClient1, string idClient2);
        
        List<IMessage> GetClientHistoryTextMessages(string idClient1, string idClient2);
        
        List<IMessage> GetClientHistoryVoiceMessages(string idClient1, string idClient2);
        
        List<IMessage> GetClientHistoryFileMessages(string idClient1, string idClient2);
    }
}