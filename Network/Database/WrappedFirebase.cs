using System;
using System.Collections.Generic;
using System.Linq;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Network.Message;

namespace Network.Database
{
    public abstract class IUser
    {
        public string Username { get; }
        public string Id { get; }
    }

    public class User : IUser
    {
        public string Username { get; }
        public string Password { get; }
        public string Id { get; }
        public List<IChat> Chats { get; } 
        public User(string username, string password, string id)
        {
            Username = username;
            Password = password;
            Id = id;
        }
    }

    public abstract class IChat
    {
        public List<IMessage> Messages { get;}
        public void AddMessage(IMessage message) => Messages.Add(message);
        public void AddMember(IUser user) => Members.Add(user);
        public List<IUser> Members { get; }
        public int Id { get; }
    }

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
    public class WrappedFirebase
    {
        private IFirebaseClient client { get; }

        public void SetChat(IChat chat)
        {
            client.Set("chats/" + chat.Id, chat);
        }

        public User GetUser(string userId)
        {
            var response = client.Get("users/" + userId);
            if (response == null) throw new ArgumentException("User not found");
            var user = response.ResultAs<User>();
            return user;
        }
        
        public void SetUser(IUser user) => client.Set("users/" + user.Id, user); 
        public IChat GetChat(string chatId)
        {
            var response= client.Get("chats/" + chatId);
            if (response == null) throw new ArgumentException("Chat not found");
            var chat = response.ResultAs<IChat>();
            return chat;
        }

        public void SetMessage(IMessage message)
        {
            throw new NotImplementedException();
        }
        
        public WrappedFirebase()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "9gsNW2lkGKAs45zVdlfVFRjRp2R2RXKwBfD9RPMS",
                BasePath = "https://chatdatabase-318d4-default-rtdb.firebaseio.com/"
            };
            client = new FirebaseClient(config);
        }
    }
    public class ClientDataBase
    {
        private WrappedFirebase db { get; }
        public ClientDataBase()
        {
            db = new WrappedFirebase();
        }
        public List<IMessage> GetMessages(string id) => db.GetChat(id).Messages;
    }
}