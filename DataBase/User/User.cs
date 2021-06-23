using System.Collections.Generic;
using DataBase.Chat;

namespace DataBase.User
{
    public class User
    {
        public string Username { get; }
        public string Password { get; }
        public string Id { get; }
        public List<Chat.Chat> Chats { get; } 
        public User(string username, string password, string id)
        {
            Username = username;
            Password = password;
            Id = id;
            Chats = new List<Chat.Chat>();
        }
    }
}