using System.Collections.Generic;
using DataBase.Chat;

namespace DataBase.User
{
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
}