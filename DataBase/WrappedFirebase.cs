using System;
using System.Collections.Generic;
using System.Linq;
using DataBase.Chat;
using DataBase.User;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Network.Message;
using Newtonsoft.Json;

namespace DataBase
{
   
    public class WrappedFirebase
    {
        private IFirebaseClient client { get; }

        public void SetChat(IChat chat)
        {
            client.Set("chats/" + chat.Id, chat);
        }

        public User.User GetUser(string userId)
        {
            var response = client.Get("users/" + userId);
            var user = response?.ResultAs<User.User>();
            return user;
        }

        public List<IUser> GetAllUsers()
        {
            var response= client.Get("users/");
            if (response.Body == "null") return new List<IUser>();
            var usersDict = JsonConvert.DeserializeObject<Dictionary<string, IUser>>(response.Body);
            return usersDict.Values.ToList();
        }

        public List<IChat> GetAllChats()
        {
            var response= client.Get("chats/");
            if (response.Body == "null") return new List<IChat>();
            var usersDict = JsonConvert.DeserializeObject<Dictionary<string, IChat>>(response.Body);
            return usersDict.Values.ToList();
        }
        
        public void SetUser(IUser user) => client.Set("users/" + user.Id, user); 
        public IChat GetChat(string chatId)
        {
            var response= client.Get("chats/" + chatId);
            var chat = response?.ResultAs<IChat>();
            return chat;
        }

        public void SetMessage(IMessage message, string chatId) => client.Set("chats/" + chatId, message);
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
}