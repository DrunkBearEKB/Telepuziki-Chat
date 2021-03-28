using System;
using System.Net.Sockets;
using System.Text;

using NetworkLibrary.Common.Package;

namespace NetworkLibrary
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
            stream.Write(package.Data, 0, package.Data.Length);
            stream.Flush();
        }

        public static byte[] GetBytes(this Encoding encoding, string str, int amount, byte defaultValue = default)
        {
            byte[] result = new byte[amount];
            byte[] temp = encoding.GetBytes(str);
            
            if (temp.Length > amount)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = temp[i];
            }

            for (int i = temp.Length; i < amount; i++)
            {
                result[i] = defaultValue;
            }

            return result;
        }
    }
}