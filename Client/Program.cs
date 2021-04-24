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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($">>> [{message.IdAuthor}] [{message.Content}] [{message.Time}]");
                Console.ForegroundColor = ConsoleColor.White;
            };
            client.OnDisconnectFromServer += () =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($">>> Disconnected!");
                Console.ForegroundColor = ConsoleColor.White;
            };

            new Thread(() => ReadFromConsole(client)).Start();
            await client.Start();
        }

        private static async void ReadFromConsole(ClientObject client)
        {
            while (true)
            {
                // ReSharper disable once PossibleNullReferenceException
                Console.ForegroundColor = ConsoleColor.White;
                string line = Console.ReadLine().TrimStart().TrimEnd();
                string[] inputParsed = line.Split();

                if (inputParsed.Length == 1)
                {
                    if (inputParsed[0].ToLower() == "disconnect")
                    {
                        await client.Disconnect();
                    }
                    else if (inputParsed[0].ToLower() == "reconnect")
                    {
                        await client.Start();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[EXCEPTION] Unhandled command: \"{inputParsed[0]}\"!");
                    }
                }
                else if (inputParsed.Length >= 2)
                {
                    await client.SendText(inputParsed[0], string.Join(" ", inputParsed.Skip(1)));
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}