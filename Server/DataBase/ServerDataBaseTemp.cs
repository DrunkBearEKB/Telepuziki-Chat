using System;
using System.Collections.Generic;
using System.Linq;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Server.DataBase
{
    public class ServerDataBaseTemp : IServerDataBase
    {
        private Dictionary<Tuple<string, string>, List<IMessage>> dictMessages;
        private HashSet<string> clients;
        
        public ServerDataBaseTemp()
        {
            this.dictMessages = new Dictionary<Tuple<string, string>, List<IMessage>>();
            this.dictMessages.Add(new Tuple<string, string>("grisha", "julia"), new List<IMessage>()
            {
                new TextMessage("julia", "grisha", new DateTime(2021, 5, 22), "test11"),
                new TextMessage("julia", "grisha", new DateTime(2021, 5, 22), "test12"),
                new TextMessage("grisha", "julia", new DateTime(2021, 5, 22), "test13"),
                new TextMessage("julia", "grisha", new DateTime(2021, 5, 22), "test14")
            });
            this.dictMessages.Add(new Tuple<string, string>("grisha", "vova"), new List<IMessage>()
            {
                new TextMessage("vova", "grisha", new DateTime(2021, 5, 22), "test21"),
                new TextMessage("vova", "grisha", new DateTime(2021, 5, 22), "test22"),
                new TextMessage("grisha", "vova", new DateTime(2021, 5, 22), "test23")
            });
            this.dictMessages.Add(new Tuple<string, string>("artem", "grisha"), new List<IMessage>()
            {
                new TextMessage("grisha", "artem", new DateTime(2021, 5, 22), "test31"),
                new TextMessage("artem", "grisha", new DateTime(2021, 5, 22), "test32"),
                new TextMessage("grisha", "artem", new DateTime(2021, 5, 22), "test33")
            });

            this.clients = new HashSet<string>() {"grisha", "julia", "vova", "artem", "test1", "test2", "temp"};
        }
        
        public List<IMessage> GetMessages(string idClient1, string idClient2, DateTime timeUntil)
        {
            Tuple<string, string> key = GetKey(idClient1, idClient2);

            if (this.dictMessages.ContainsKey(key))
            {
                return this.dictMessages[key].Where(message => message.Time > timeUntil).ToList();
            }

            throw new ArgumentException($"No history for users: \"{idClient1}\" and \"{idClient2}\"");
        }
        
        public void AddMessage(IMessage message)
        {
            Tuple<string, string> key = GetKey(message.IdReceiver, message.IdAuthor);
            
            if (!this.dictMessages.ContainsKey(key))
            {
                this.dictMessages.Add(key, new List<IMessage>());
            }
            
            this.dictMessages[key].Add(message);
        }
        
        public void RemoveMessage(IMessage message)
        {
            Tuple<string, string> key = GetKey(message.IdReceiver, message.IdAuthor);
            
            if (!this.dictMessages.ContainsKey(key))
            {
                throw new ArgumentException($"No history for users: \"{message.IdAuthor}\" and \"{message.IdReceiver}\"");
            }

            this.dictMessages[key].Remove(message);
        }

        public List<string> GetUsersWithSimilarId(string id)
        {
            return this.clients.Where(clientId => DamerauLevenshteinDistance(id, clientId) < 3).ToList();
        }

        public void AddClient(string id)
        {
            if (this.clients.Contains(id))
            {
                throw new ArgumentException($"Already contains such user {id}");
            }

            this.clients.Add(id);
        }

        public bool ContainsClient(string id)
        {
            return this.clients.Contains(id);
        }

        private static Tuple<string, string> GetKey(string str1, string str2)
        {
            return String.CompareOrdinal(str1, str2) < 0
                ? new Tuple<string, string>(str1, str2)
                : new Tuple<string, string>(str2, str1);
        }

        private static int Minimum(int num1, int num2) => 
            num1 < num2 ? num1 : num2;

        private static int Minimum(int num1, int num2, int num3) => (
            num1 = num1 < num2 ? num1 : num2) < num3 ? num1 : num3;

        private static int DamerauLevenshteinDistance(string string1, string string2)
        {
            int n = string1.Length + 1;
            int m = string2.Length + 1;
            int[,] arrayD = new int[n, m];

            for (int i = 0; i < n; i++)
            {
                arrayD[i, 0] = i;
            }

            for (int j = 0; j < m; j++)
            {
                arrayD[0, j] = j;
            }

            for (int i = 1; i < n; i++)
            {
                for (int j = 1; j < m; j++)
                {
                    int cost = string1[i - 1] == string2[j - 1] ? 0 : 1;

                    arrayD[i, j] = Minimum(arrayD[i - 1, j] + 1,
                        arrayD[i, j - 1] + 1,
                        arrayD[i - 1, j - 1] + cost);

                    if (i > 1 && j > 1 
                              && string1[i - 1] == string2[j - 2]
                              && string1[i - 2] == string2[j - 1])
                    {
                        arrayD[i, j] = Minimum(arrayD[i, j],
                            arrayD[i - 2, j - 2] + cost);
                    }
                }
            }

            return arrayD[n - 1, m - 1];
        }
    }
}