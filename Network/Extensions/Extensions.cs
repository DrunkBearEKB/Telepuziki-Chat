using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using Network.Package;

namespace Network.Extensions
{
    public static class Extensions
    {
        public static T[] Sub<T>(this T[] array, int indexStart)
        {
            return array.Sub(indexStart, array.Length - indexStart);
        }
        
        public static T[] Sub<T>(this T[] array, int indexStart, int amount)
        {
            if (indexStart + amount > array.Length)
            {
                throw new IndexOutOfRangeException();
            }
            
            T[] result = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                result[i] = array[indexStart + i];
            }

            return result;
        }

        public static void Write(this NetworkStream stream, IPackage package)
        {
            stream.Write(package.RawData, 0, package.RawData.Length);
            stream.Flush();
        }
        
        public static async Task WriteAsync(this NetworkStream stream, IPackage package)
        {
            await stream.WriteAsync(package.RawData, 0, package.RawData.Length);
            await stream.FlushAsync();
        }
    }
}