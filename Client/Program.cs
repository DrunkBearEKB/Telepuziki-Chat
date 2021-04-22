using System;
using System.Collections.Generic;
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

            ClientObject client = new ClientObject(Console.ReadLine());

            client.OnTextMessageReceive += message => Console.WriteLine($">>> [{message.IdAuthor} {message.Content}]");
            //client.OnOnlineChecking += () => Console.WriteLine($">>> Online checked!");
            client.OnDisconnectFromServerForced += () => Console.WriteLine($">>> Disconnected!");

            new Thread(new ThreadStart(() => Reading(client))).Start();
            await client.Start();
        }

        static void Reading(ClientObject client)
        {
            while (true)
            {
                // ReSharper disable once PossibleNullReferenceException
                Console.Write("<<< ");
                string[] inputParsed = Console.ReadLine().Split();

                if (inputParsed.Length == 1)
                {
                    client.SendText("", inputParsed[0]);
                }
                else if (inputParsed.Length >= 2)
                {
                    client.SendText(inputParsed[0], string.Join(" ", inputParsed.Skip(1)));
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}