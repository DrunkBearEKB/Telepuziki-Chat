using System;
using System.Collections.Generic;
using System.Linq;
using DataBase.Chat;
using DataBase.User;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Network.Message;
using Network.Message.ExchangingMessages;
using Newtonsoft.Json;

namespace DataBase
{
   
    public class WrappedFirebase
    {
        private IFirebaseClient client { get; }

        public void SetChat(Chat.Chat chat)
        {
            client.Set("chats/" + chat.Id, chat);
        }

        public User.User GetUser(string userId)
        {
            var response = client.Get($"users/{userId}");
            return response?.ResultAs<User.User>();
        }

        public List<User.User> GetAllUsers()
        {
            var response = client.Get("users/");
            if (response.Body == "null") 
                return new List<User.User>();
            return JsonConvert.DeserializeObject<Dictionary<string, User.User>>(response.Body).Values.ToList();
        }

        public List<Chat.Chat> GetAllChats()
        {
            var response = client.Get("chats/");
            if (response.Body == "null") 
                return new List<Chat.Chat>();
            return JsonConvert.DeserializeObject<Dictionary<string, Chat.Chat>>(response.Body).Values.ToList();
        }
        
        public void SetUser(User.User user) => client.Set($"users/{user.Id}", user);
        public Chat.Chat GetChat(string chatId)
        {
            var response = client.Get($"chats/{chatId}");
            return response?.ResultAs<Chat.Chat>();
        }

        public void SetMessage(TextMessage message, string chatId)
        {
            var response = client.Get($"chats/{chatId}/");
            if (response != null)
            {
                var chat = response.ResultAs<Chat.Chat>();
                chat.Messages.Add(message);
                SetChat(chat);
            }
        }

        public WrappedFirebase()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "KbWpEtBEc4rnLSNKsJEysQ3kb7QMDrB0Ht8Dws42",
                BasePath = "https://chat-a0ca0-default-rtdb.firebaseio.com/"
            };
            client = new FirebaseClient(config);
        }
    }
}