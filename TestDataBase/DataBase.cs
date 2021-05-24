using System.Collections.Generic;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Network.Message;

namespace GDataBase
{
    public class DataBase : IDataBase
    {
        private IFirebaseClient client { get; }
        
        public DataBase()
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "AIzaSyDh_Z7TqQZJcaRqHYkF_tNlZOGdP9aXmJM",
                BasePath = "https://chatdatabase-318d4-default-rtdb.firebaseio.com/"
            };
            this.client = new FirebaseClient(config);
        }
        
        public async Task<List<string>> GetListClientsId()
        {
            FirebaseResponse response = await this.client.GetAsync("");

            if (response.Body == "null")
            {
                return new List<string>();
            }
            
            throw new System.NotImplementedException();
        }

        public Task<List<IMessage>> GetMessages(string id1, string id2)
        {
            throw new System.NotImplementedException();
        }

        public void AddClient(string id)
        {
            throw new System.NotImplementedException();
        }

        public void AddMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}