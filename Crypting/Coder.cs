using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Crypting
{
    public class Coder : ICryptable
    {
        private readonly string passPhrase;
        private readonly string saltValue;
        private readonly string hashAlgorithm;
        private readonly int passwordIterations;
        private readonly string initVector;
        private readonly int keySize;

        public Coder(string passPhrase,
            string saltValue = "YoungTrazyCrazyVEtoy",
            string hashAlgorithm = "SHA256",
            int passwordIterations = 2,
            string initVector = "!2B6g2D4t9F526g7",
            int keySize = 256)
        {
            this.passPhrase = passPhrase;
            // любая строка
            this.saltValue = saltValue;
            // любая строка
            this.hashAlgorithm = hashAlgorithm;
            //"MD5" или "SHA256"
            this.passwordIterations = passwordIterations;
            // любое число
            this.initVector = initVector;
            // последовательность в 16 байт
            this.keySize = keySize;
            // 128 или 256
        }
        public byte[] Encrypt(byte[] dataBytes)
        {

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);


            PasswordDeriveBytes password = new(
                passPhrase,
                saltValueBytes,
                hashAlgorithm,
                passwordIterations);


            byte[] keyBytes = password.GetBytes(keySize / 8);

            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor
            (
                keyBytes,
                initVectorBytes
            );

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream
            (
                memoryStream,
                encryptor,
                CryptoStreamMode.Write
            );

            cryptoStream.Write(dataBytes, 0, dataBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();
            return cipherTextBytes;

        }

        public byte[] Decrypt(byte[] cipherTextBytes)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            PasswordDeriveBytes password = new PasswordDeriveBytes
            (
                passPhrase,
                saltValueBytes,
                hashAlgorithm,
                passwordIterations
            );

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new();


            symmetricKey.Mode = CipherMode.CBC;


            ICryptoTransform decryptor = symmetricKey.CreateDecryptor
            (
                keyBytes,
                initVectorBytes
            );

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream
            (
                memoryStream,
                decryptor,
                CryptoStreamMode.Read
            );
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            _ = cryptoStream.Read
            (
                plainTextBytes,
                0,
                plainTextBytes.Length
            );

            memoryStream.Close();
            cryptoStream.Close();

            return plainTextBytes;
        }

        private byte[] DeleteEmptyEnd(byte[] bytes)
        {
            var result = new List<byte>();
            var copy = new byte[bytes.Length];
            Array.Copy(bytes, copy, bytes.Length);
            Array.Reverse(copy);
            var zeroCounter = 0;
            foreach (var b in copy)
                if (b is default(byte))
                    zeroCounter++;
            for (int i = 0; i < bytes.Length - zeroCounter; i++)
                result.Add(bytes[i]);
            return result.ToArray();
        }
    }
}
