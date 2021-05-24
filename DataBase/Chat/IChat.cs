using System.Collections.Generic;
using DataBase.User;
using Network.Message;

namespace DataBase.Chat
{
    public abstract class IChat
    {
        public List<IMessage> Messages { get;}
        public void AddMessage(IMessage message) => Messages.Add(message);
        public void AddMember(IUser user) => Members.Add(user);
        public List<IUser> Members { get; }
        public int Id { get; }
    }
}