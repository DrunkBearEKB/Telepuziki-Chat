using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Network;
using Network.Message;

namespace Client.UserInteface
{
    public class MyConsole
    {
        private string idClient;
        private ClientObject clientObject;
        private List<Action> listUnhandledActions = new List<Action>();

        private Thread threadReadFromConsole;
        
        private static ConsoleColor colorInput = ConsoleColor.White;
        private static ConsoleColor colorReceiveMessage = ConsoleColor.Green;
        private static ConsoleColor colorWarning = ConsoleColor.Yellow;
        private static ConsoleColor colorException = ConsoleColor.Red;
        
        public void GetClientId()
        {
            Console.Write("Enter your name: ");
            this.idClient = Console.ReadLine();
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine($"You are logged in as a {this.idClient}!\n" +
                              $"Now you can send messages in the format: <receiver> <message>");
            
            this.CreateNetworkConnection();
        }

        private void CreateNetworkConnection()
        {
            this.clientObject = new ClientObject(this.idClient) { AutoReconnect = false };

            this.clientObject.OnTextMessageReceive += message =>
            {
                string text = $">>> from: {message.IdAuthor} \"{message.Content}\" [{message.Time}]";
                
                if (!Console.KeyAvailable)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.ForegroundColor = colorReceiveMessage;
                    Console.WriteLine(text);
                    Console.ForegroundColor = colorInput;
                }
                else
                {
                    this.listUnhandledActions.Add(() =>
                    {
                        Console.ForegroundColor = colorReceiveMessage;
                        Console.WriteLine(text);
                        Console.ForegroundColor = colorInput;
                    });
                }
            };
            this.clientObject.OnDisconnectFromServer += () =>
            {
                string text = ">>> Disconnected!";
                
                if (!Console.KeyAvailable)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.ForegroundColor = colorWarning;
                    Console.WriteLine(text);
                    Console.ForegroundColor = colorInput;
                }
                else
                {
                    this.listUnhandledActions.Add(() =>
                    {
                        Console.ForegroundColor = colorWarning;
                        Console.WriteLine(text);
                        Console.ForegroundColor = colorInput;
                    });
                }
            };
        }

        public async Task Start()
        {
            this.threadReadFromConsole = new Thread(ReadFromConsole);
            this.threadReadFromConsole.Start();
            
            await this.clientObject.Start();
        }

        private async void ReadFromConsole()
        {
            while (true)
            {
                Console.ForegroundColor = colorInput;

                string line = Console.ReadLine().TrimStart().TrimEnd();
                Console.SetCursorPosition(0, Console.CursorTop - 1);

                string[] inputParsed = line.Split();

                if (inputParsed.Length == 1)
                {
                    if (inputParsed[0].ToLower() == "connect")
                    {
                        Console.ForegroundColor = colorInput;
                        Console.WriteLine($"<<< [COMMAND] {line}");
                        Console.ForegroundColor = colorException;
                        Console.WriteLine(">>> [EXCEPTION] Unable to reconnect, please restart the client.");
                        return;
                    }
                    else if (inputParsed[0].ToLower() == "disconnect")
                    {
                        Console.WriteLine($"<<< [COMMAND] {line}");
                        await this.clientObject.Disconnect();
                    }
                    else
                    {
                        if (inputParsed[0].Length != 0)
                        {
                            Console.ForegroundColor = colorException;
                            Console.WriteLine($">>> [EXCEPTION] Unhandled command: \"{inputParsed[0]}\"!");
                        }
                        else if (listUnhandledActions.Count == 0)
                        {
                            Console.WriteLine(">>>");
                        }
                    }
                }
                else if (inputParsed.Length >= 2)
                {
                    string text = string.Join(" ", inputParsed.Skip(1));
                    Console.WriteLine($"<<< to: {inputParsed[0]} \"{text}\" [{DateTime.Now}]");
                    await this.clientObject.SendText(inputParsed[0], text);
                }
                
                Console.ForegroundColor = colorInput;

                foreach (Action action in listUnhandledActions)
                {
                    action();
                }
                listUnhandledActions.Clear();
                //CanPrintInConsole = true;
            }
        }
    }
}