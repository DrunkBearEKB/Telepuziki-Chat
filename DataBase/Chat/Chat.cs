using System.Collections.Generic;
using DataBase.User;
using Network.Message;

namespace DataBase.Chat
{
    public class Chat : IChat
    {
        public string Id { get; }
        public string ChatTitle { get; }
        public List<IUser> Members;
        public List<IMessage> Messages;
        public Chat(string id, string chatTitle, List<IUser> members)
        {
            Id = id;
            ChatTitle = chatTitle;
            Members = members;
        }

        public List<IMessage> GetAllMessages()
        {
            return Messages;
            // Messages.Select(m => m.Text)
        }
    }
}