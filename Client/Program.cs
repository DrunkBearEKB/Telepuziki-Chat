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
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        static async Task Main()
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Console.Write("Enter your name: ");
            ClientObject client = new ClientObject(Console.ReadLine());

            client.OnTextMessageReceive += message => 
                Console.WriteLine($">>> [{message.IdAuthor}] [{message.Content}] [{message.Time}]");
            client.OnDisconnectFromServer += () => 
                Console.WriteLine($">>> Disconnected!");

            new Thread(() => ReadFromConsole(client)).Start();
            await client.Start();
        }

        private static async void ReadFromConsole(ClientObject client)
        {
            while (true)
            {
                // ReSharper disable once PossibleNullReferenceException
                string[] inputParsed = Console.ReadLine().Split();

                if (inputParsed.Length == 1)
                {
                    if (inputParsed[0].ToLower() == "disconnect")
                    {
                        await client.Disconnect();
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