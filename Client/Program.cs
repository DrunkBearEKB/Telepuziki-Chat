using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

using Client.Network;

namespace Client
{
    static class Program
    {
        private static Thread threadReadFromConsole;

        private static ConsoleColor colorInput = ConsoleColor.White;
        private static ConsoleColor colorReceiveMessage = ConsoleColor.Green;
        private static ConsoleColor colorWarning = ConsoleColor.Yellow;
        private static ConsoleColor colorException = ConsoleColor.Red;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static async Task Main()
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Console.Write("Enter your name: ");
            ClientObject client = new ClientObject(Console.ReadLine()) { AutoReconnect = false};

            client.OnTextMessageReceive += message =>
            {
                Console.ForegroundColor = colorReceiveMessage;
                Console.WriteLine($">>> from: {message.IdAuthor} \"{message.Content}\" [{message.Time}]");
                Console.ForegroundColor = colorInput;
            };
            client.OnDisconnectFromServer += () =>
            {
                Console.ForegroundColor = colorWarning;
                Console.WriteLine(">>> Disconnected!");
                Console.ForegroundColor = colorInput;
            };
            
            threadReadFromConsole = new Thread(() => ReadFromConsole(client));
            threadReadFromConsole.Start();

            await client.Start();
        }

        private static async void ReadFromConsole(ClientObject client)
        {
            while (true)
            {
                Console.ForegroundColor = colorInput;
                string line = Console.ReadLine();
                Console.SetCursorPosition(0, Console.CursorTop - 1);

                line?.TrimStart().TrimEnd();
                
                if (line != null)
                {
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
                            await client.Disconnect();
                        }
                        else
                        {
                            Console.ForegroundColor = colorException;
                            Console.WriteLine($">>> [EXCEPTION] Unhandled command: \"{inputParsed[0]}\"!");
                        }
                    }
                    else if (inputParsed.Length >= 2)
                    {
                        string text = string.Join(" ", inputParsed.Skip(1));
                        Console.WriteLine($"<<< to: {inputParsed[0]} \"{text}\" [{DateTime.Now}]");
                        await client.SendText(inputParsed[0], text);
                    }
                }
                Console.ForegroundColor = colorInput;
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}