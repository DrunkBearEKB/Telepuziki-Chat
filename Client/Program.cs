using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

using Client.Network;
using Client.UserInterface;

namespace Client
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());
        }
        
        /*static async Task Main()
        {
            Bitmap bmp = new Bitmap(
                Image.FromFile(@"D:\projects\C#\Telepuziki-Chat\Client\Resources\themeDark\profile.png"));
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color color = bmp.GetPixel(x, y);
                    if (color.R != 14 || color.G != 22 || color.B != 33)
                    {
                        color = Color.FromArgb(255, 255, 255);
                        bmp.SetPixel(x, y, color);
                    }
                }
            }
            bmp.Save("profile.png", ImageFormat.Png);
        }*/
    }
}