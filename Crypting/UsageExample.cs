using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypting
{
    class UsageExample
    {
        static void Main(string[] args)
        {
            string strokaDlyaSfifrovki = "Example String telepuziki";
            byte[] dataBytes = Encoding.Default.GetBytes(strokaDlyaSfifrovki);           

            //необязательные параметры в конструкторе Coder'a
            //string saltValue = "TestSaltValue";
            //Может быть любой строкой
            //string hashAlgorithm = "SHA256";
            //может быть "MD5"
            //int passwordIterations = 2;
            //Может быть любым числом
            //string initVector = "!1A3g2D4s9K556g7";
            //Должно быть 16 байт
            //int keySize = 256;
            //Может быть 192 или 128


            var coder = new Coder("password1488");
            var cipherTextBytes = coder.Encrypt(dataBytes);
            var bytes = coder.Decrypt(cipherTextBytes);
            
            string oldText = Encoding.Default.GetString
            (
                bytes,
                0,
                bytes.Length
            );
            Console.WriteLine(oldText);
            Console.ReadKey();
        }
    }
}
