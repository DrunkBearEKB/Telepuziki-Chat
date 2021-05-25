using System.Collections.Generic;
using Network.Message;

namespace Client.UserInterface
{
    public interface IClientForm
    {
        void ChangeChat(string idPrevious);

        void AddMessage(string idSender, IMessage message);

        void AddContactQuestion(string id);

        Dictionary<string, List<IMessage>> GetHistory();

        void SetHistory(string id, List<IMessage> messages);

        Dictionary<string, List<IMessage>> dictMessageHistory { get; }
    }
}