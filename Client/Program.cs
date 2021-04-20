using System;
using System.Collections.Generic;
using System.Linq;
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
            await client.Start();

            while (true)
            {
                // ReSharper disable once PossibleNullReferenceException
                string[] inputParsed = Console.ReadLine().Split();

                if (inputParsed.Length == 1)
                {
                    await client.SendMessage("", inputParsed[0]);
                }
                else if (inputParsed.Length >= 2)
                {
                    await client.SendMessage(inputParsed[0], string.Join(" ", inputParsed.Skip(1)));
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}