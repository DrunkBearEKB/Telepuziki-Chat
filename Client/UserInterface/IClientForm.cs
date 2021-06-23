using System.Collections.Generic;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Client.UserInterface
{
    public interface IClientForm
    {
        void ChangeChat(string idPrevious);

        void AddMessage(string idSender, TextMessage message);

        void AddContactQuestion(string id);

        Dictionary<string, List<TextMessage>> GetHistory();

        void SetHistory(string id, List<TextMessage> messages);

        Dictionary<string, List<TextMessage>> dictMessageHistory { get; }
    }
}