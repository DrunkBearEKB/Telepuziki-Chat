using System.Threading.Tasks;
using Client.UserInteface;

namespace Client
{
    static class Program
    {
        static async Task Main()
        {
            MyConsole console = new MyConsole();
            console.GetClientId();
            await console.Start();
        }
    }
}