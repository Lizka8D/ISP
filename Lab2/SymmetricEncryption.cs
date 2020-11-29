using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Lab2
{
    public class SymmetricEncryption
    {
        public static string Encrypt
        (
            string textFromFile,
            string passPhrase,
            string saltValue,
            string hashAlgorithm,
            int passwordIterations,
            string initVector,
            int keySize
        )
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(textFromFile);

            PasswordDeriveBytes password = new PasswordDeriveBytes
            (
                passPhrase,
                saltValueBytes,
                hashAlgorithm,
                passwordIterations
            );

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();
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

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }
        public static string Decrypt
        (
            string cipherText,
            string passPhrase,
            string saltValue,
            string hashAlgorithm,
            int passwordIterations,
            string initVector,
            int keySize
        )
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes
            (
                passPhrase,
                saltValueBytes,
                hashAlgorithm,
                passwordIterations
            );

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

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

            int decryptedByteCount = cryptoStream.Read
            (
                plainTextBytes,
                0,
                plainTextBytes.Length
            );

            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.Default.GetString
            (
                plainTextBytes,
                0,
                decryptedByteCount
            );

            return plainText;
        }
    }
}
