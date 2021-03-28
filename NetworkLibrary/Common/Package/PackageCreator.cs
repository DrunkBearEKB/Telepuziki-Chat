using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetworkLibrary.Common.Package.ExchangingPackages;

namespace NetworkLibrary.Common.Package
{
    public class PackageCreator
    {
        public static readonly Encoding Encoding = Encoding.Unicode;
        public static readonly int AmountBytesForSymbol = Encoding.GetBytes("1").Length;
        public static readonly int AmountBytesId = AmountBytesForSymbol * 16;
        public static readonly int AmountBytesTime = AmountBytesForSymbol * 32;
        
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
            
            int amountBytes = BitConverter.ToInt32(this.bytesBuffer.Sub(1, 4));
            if (amountBytes + 5 <= this.amountBytesBufferFullness)
            {
                byte byteFirst = this.bytesBuffer.FirstOrDefault();

                IPackage package = byteFirst switch
                {
                    1 => new TextPackage(this.bytesBuffer.Sub(5 + amountBytes)),
                    2 => new VoicePackage(this.bytesBuffer.Sub(5 + amountBytes)),
                    3 => new FilePackage(this.bytesBuffer.Sub(5 + amountBytes)),
                    5 => new OnlinePackage(this.bytesBuffer.Sub(5 + amountBytes)),
                    6 => new DisconnectPackage(this.bytesBuffer.Sub(5 + amountBytes)),
                    7 => new HistoryRequestPackage(this.bytesBuffer.Sub(5 + amountBytes)),
                    8 => new HistoryAnswerPackage(this.bytesBuffer.Sub(5 + amountBytes)),
                    _ => null
                };

                this.queuePackagesReady.Enqueue(package);
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
    }
}