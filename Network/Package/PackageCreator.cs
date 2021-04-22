using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Network.Extensions;
using Network.Package.ExchangingPackages;

namespace Network.Package
{
    public class PackageCreator
    {
        public static readonly Encoding Encoding = Encoding.Unicode;

        private readonly byte[] bytesBuffer;
        private int amountBytesBufferFullness = 0;
        private readonly Queue<IPackage> queuePackagesReady;
        
        public PackageCreator(int bufferSize = 1024)
        {
            this.bytesBuffer = new byte[bufferSize];
            this.queuePackagesReady = new Queue<IPackage>();
        }

        public void Add(byte[] bytes, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                this.bytesBuffer[amountBytesBufferFullness + i] = bytes[i];
            }
            this.amountBytesBufferFullness += amount;
            
            int amountBytes = BitConverter.ToInt32(this.bytesBuffer.Sub(0, 4));

            if (this.amountBytesBufferFullness >= amountBytes)
            {
                byte byteType = this.bytesBuffer.Sub(4, 1).FirstOrDefault();
                byte[] bytesForPackage = this.bytesBuffer.Sub(0, 4 + amountBytes);

                IPackage package = byteType switch
                {
                    1 => new TextPackage(bytesForPackage),
                    2 => new VoicePackage(bytesForPackage),
                    3 => new FilePackage(bytesForPackage),
                    5 => new OnlinePackage(bytesForPackage),
                    6 => new DisconnectPackage(bytesForPackage),
                    7 => new HistoryRequestPackage(bytesForPackage),
                    8 => new HistoryAnswerPackage(bytesForPackage),
                    _ => null
                };

                if (package != null)
                {
                    this.queuePackagesReady.Enqueue(package);
                    this.amountBytesBufferFullness -= amountBytes + 4;
                }
            }
        }
        
        public bool CanGetPackage => this.queuePackagesReady.Count != 0;

        public IPackage GetPackage()
        {
            if (!this.CanGetPackage)
            {
                throw new NullReferenceException();
            }

            return this.queuePackagesReady.Dequeue();
        }

        public static byte[] GetRawFormattedData(PackageType type, params string[] arrayString)
        {
            byte[] result = arrayString
                .Select(str => PackageCreator.Encoding.GetBytes(str))
                .Select(arrayByte => (new byte[] { (byte) arrayByte.Length }).Concat(arrayByte).ToArray())
                .SelectMany(arrayByte => arrayByte)
                .ToArray();
            result = new[] { (byte)type }
                .Concat(result)
                .ToArray();
            return BitConverter.GetBytes(result.Length).Concat(result).ToArray();
        }

        public static byte[] GetRawFormattedData(PackageType type, string[] arrayString, byte[] bytes)
        {
            byte[] result = arrayString
                .Select(str => PackageCreator.Encoding.GetBytes(str))
                .Select(arrayByte => (new byte[] { (byte) arrayByte.Length }).Concat(arrayByte).ToArray())
                .SelectMany(arrayByte => arrayByte)
                .Concat(bytes)
                .ToArray();
            result = new[] { (byte)type }
                .Concat(result)
                .ToArray();
            return BitConverter.GetBytes(result.Length).Concat(result).ToArray();
        }

        public static List<byte[]> Parse(byte[] bytes)
        {
            bytes = bytes.Sub(5);
            
            List<byte[]> result = new List<byte[]>();

            for (int i = 0; i < bytes.Length; i++)
            {
                int length = bytes[i];
                result.Add(bytes.Sub(i + 1, length));
                i += length;
            }

            return result;
        }

        public static string GetFormattedTime(DateTime dateTime)
        {
            return dateTime.ToFileTimeUtc().ToString();
        }
        
        public static DateTime ParseTime(byte[] bytes)
        {
            return DateTime.FromFileTimeUtc(Convert.ToInt64(PackageCreator.Encoding.GetString(bytes)));
        }
    }
}