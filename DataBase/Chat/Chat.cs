using System.Collections.Generic;
using DataBase.User;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace DataBase.Chat
{
    public class Chat
    {
        public string Id { get; }
        public string ChatTitle { get; }
        public List<User.User> Members { get; }
        public List<TextMessage> Messages { get; }

        public Chat(string id, string chatTitle, List<User.User> members, List<TextMessage> messages)
        {
            Id = id;
            ChatTitle = chatTitle;
            Members = members;
            Messages = messages;
        }

        public List<TextMessage> GetAllMessages()
        {
            return Messages;
            // Messages.Select(m => m.Text)
        }
    }
}