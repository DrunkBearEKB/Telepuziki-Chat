using System.Collections.Generic;
using Network.Message;

namespace Client.DataBase
{
    public interface IClientDataBase
    {
        List<IMessage> GetMessages(string id); // id - id человека, с которым подгружается история
    }
}