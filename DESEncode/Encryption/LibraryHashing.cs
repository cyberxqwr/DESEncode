using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DESEncode.Encryption
{
    public class LibraryHashing : IDisposable
    {

        private const int BlockSize = 64;
        private string messageValue;
        private string keyValue;
        private byte[] ciphertext;
        private byte[] iv;

        private List<string> bits64 = new List<string>();

        private bool is_disposed = false;

        public LibraryHashing(string message, string key)
        {

            messageValue = message;
            keyValue = key;
            RegulateEncryption();
        }

        public LibraryHashing(byte[] ciphertext, byte[] iv, string key)
        {

            this.ciphertext = ciphertext;
            this.iv = iv;
            keyValue = key;
            RegulateEncryption();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.is_disposed = true;
        }

        public void EncryptCbcUser()
        {

            byte[] iv = GenerateRandomIV();
            string cryptedBytes = "";

            Console.WriteLine("Naudojamas IV:");

            foreach (byte b in iv)
            {
                Console.Write(b + " ");
                cryptedBytes += b + " ";
            }

            Console.WriteLine("\nEncrypted bytes: ");

            cryptedBytes += "-";

            foreach (string text in bits64)
            {

                byte[] bytes = EncryptCbc(text, iv);

                foreach (byte b in bytes)
                {

                    Console.Write(b + " ");
                    cryptedBytes += b + " ";
                }

            }

            Console.WriteLine("\n\nAr norite irasyti i faila?");
            Console.WriteLine("[Y] - Taip / [N] - Ne");
            string input;
            input = Console.ReadLine();
            do
            {

                if (input == "Y" || input == "y")
                {

                    FileReader.WriteEncryptedMessageToFile("CBCEnc.txt", cryptedBytes);
                    Console.WriteLine("\n\nFailas irasytas\n\n");
                    break;
                }

            } while (input != "N" && input != "n");

        }

        public string DecryptCbcUser()
        {

            byte[] tempCiphertext = new byte[16];
            string cipherText = "";
            byte[] decryptedBytes;
            List<byte> bytes = new List<byte>();

            if (ciphertext.Count() > 16)
            {

                for (int i = 0; i < ciphertext.Count(); i++)
                {

                    bytes.Add(ciphertext[i]);
                    if (i == 15)
                    {
                        tempCiphertext = bytes.ToArray();
                        decryptedBytes = DecryptCbc(tempCiphertext);
                        cipherText += Encoding.UTF8.GetString(decryptedBytes);
                        bytes.Clear();
                    }

                }
            }
            else
            {

                decryptedBytes = DecryptCbc(ciphertext);
                cipherText += Encoding.UTF8.GetString(decryptedBytes);
            }

            return cipherText;
        }

        private byte[] GenerateRandomIV()
        {
            const int BlockSize = 64;

            using (var generator = RandomNumberGenerator.Create())
            {
                byte[] iv = new byte[BlockSize / 8];
                generator.GetBytes(iv);
                return iv;
            }
        }

        private byte[] EncryptCbc(string text, byte[] iv)
        {

            using (var des = new DESCryptoServiceProvider())
            {

                byte[] message = ConvertBits(text);
                byte[] key = ConvertBits(keyValue);

                message = PadMessage(message, PaddingMode.PKCS7);

                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.IV = iv;
                des.Key = key;

                using (var encryptor = des.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(message, 0, message.Length);
                }
            }
        }

        private void RegulateEncryption()
        {

            StringBuilder sb = new StringBuilder();

            if (messageValue != null)
            {
                if (messageValue.Length % 64 != 0)
                {

                    while (messageValue.Length % 64 != 0)
                    {

                        sb.Append("0");
                        messageValue += sb.ToString();
                        sb.Clear();
                    }

                }

                sb.Clear();

                for (int i = 0; i < messageValue.Length; i += 64) bits64.Add(messageValue.Substring(i, 64));
            }

            if (keyValue.Length % 64 != 0)
            {

                while (keyValue.Length % 64 != 0)
                {

                    sb.Append("0");
                    keyValue += sb.ToString();
                    sb.Clear();
                }
            }
        }

        private byte[] DecryptCbc(byte[] cipher)
        {

            using (var des = new DESCryptoServiceProvider())
            {
                byte[] key = ConvertBits(keyValue);

                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.IV = iv;
                des.Key = key;

                using (var decryptor = des.CreateDecryptor())
                {
                    var plaintext = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                    return UnpadMessage(plaintext, PaddingMode.PKCS7);
                }
            }
        }

        private byte[] ConvertBits(string text)
        {

            StringBuilder sb = new StringBuilder();
            byte[] convertedBits = new byte[8];
            int counter = 0;

            for (int i = 0; i < text.Count(); i++)
            {

                sb.Append(text[i]);
                if (i % 8 == 7)
                {

                    convertedBits[counter] = Convert.ToByte(sb.ToString(), 2);
                    sb.Clear();
                    counter++;
                }
            }

            return convertedBits;
        }

        private byte[] PadMessage(byte[] message, PaddingMode paddingMode)
        {
            if (paddingMode != PaddingMode.PKCS7)
            {
                throw new ArgumentOutOfRangeException(nameof(paddingMode), "Nera PKCS#7.");
            }

            int messageLength = message.Length;
            int mod = messageLength % BlockSize / 8;
            int padBytes = mod == 0 ? BlockSize / 8 : BlockSize / 8 - mod;

            byte[] paddedMessage = new byte[messageLength + padBytes];
            Array.Copy(message, 0, paddedMessage, 0, messageLength);

            byte padValue = (byte)padBytes;
            for (int i = messageLength; i < paddedMessage.Length; i++)
            {
                paddedMessage[i] = padValue;
            }

            return paddedMessage;
        }

        private byte[] UnpadMessage(byte[] message, PaddingMode paddingMode)
        {
            if (paddingMode != PaddingMode.PKCS7)
            {
                throw new ArgumentOutOfRangeException(nameof(paddingMode), "Nera PKCS#7.");
            }

            if (message.Length == 0)
            {
                return message;
            }

            int padValue = message[message.Length - 1];

            if (padValue < 1 || padValue > BlockSize / 8)
            {
                throw new CryptographicException("Neteisingas paddingas.");
            }

            int unpaddedLength = message.Length - padValue;
            if (unpaddedLength < 0)
            {
                throw new CryptographicException("Neteisingas ilgis.");
            }

            for (int i = unpaddedLength; i < message.Length; i++)
            {
                if (message[i] != padValue)
                {
                    throw new CryptographicException("Neteisingas paddingas.");
                }
            }

            byte[] unpaddedMessage = new byte[unpaddedLength];
            Array.Copy(message, 0, unpaddedMessage, 0, unpaddedLength);

            return unpaddedMessage;
        }

        public void EncryptCfbUser()
        {
            string cryptedBytes = "";
            byte[] iv = GenerateRandomIV();

            Console.WriteLine("Naudojamas IV:");

            foreach (byte b in iv)
            {
                Console.Write(b + " ");
                cryptedBytes += b + " ";
            }

            Console.WriteLine("\nEncrypted bytes: ");

            cryptedBytes += "-";

            foreach (string text in bits64)
            {

                byte[] bytes = EncryptCfb(text, iv);

                foreach (byte b in bytes)
                {

                    Console.Write(b + " ");
                    cryptedBytes += b + " ";
                }

            }

            Console.WriteLine("Ar norite irasyti i faila?");
            Console.WriteLine("[Y] - Taip / [N] - Ne");
            string input;
            input = Console.ReadLine();
            do
            {

                if (input == "Y" || input == "y")
                {

                    FileReader.WriteEncryptedMessageToFile("CFBEnc.txt", cryptedBytes);
                    Console.WriteLine("\n\nFailas irasytas\n\n");
                    break;
                }

            } while (input != "N" && input != "n");

        }

        public string DecryptCfbUser()
        {

            byte[] tempCiphertext = new byte[16];
            string cipherText = "";
            byte[] decryptedBytes;
            List<byte> bytes = new List<byte>();

            if (ciphertext.Count() > 16)
            {

                for (int i = 0; i < ciphertext.Count(); i++)
                {

                    bytes.Add(ciphertext[i]);
                    if (i == 15)
                    {
                        tempCiphertext = bytes.ToArray();
                        decryptedBytes = DecryptCfb(tempCiphertext);
                        cipherText += Encoding.UTF8.GetString(decryptedBytes);
                        bytes.Clear();
                    }

                }
            }
            else
            {

                decryptedBytes = DecryptCfb(ciphertext);
                cipherText += Encoding.UTF8.GetString(decryptedBytes);
            }

            return cipherText;
        }

        private byte[] EncryptCfb(string text, byte[] iv)
        {

            using (var des = new DESCryptoServiceProvider())
            {

                byte[] message = ConvertBits(text);
                byte[] key = ConvertBits(keyValue);

                des.Mode = CipherMode.CFB;
                des.Padding = PaddingMode.None;

                des.IV = iv;
                des.Key = key;

                using (var encryptor = des.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(message, 0, message.Length);
                }
            }
        }

        private byte[] DecryptCfb(byte[] cipher)
        {

            using (var des = new DESCryptoServiceProvider())
            {
                byte[] key = ConvertBits(keyValue);

                des.Mode = CipherMode.CFB;
                des.Padding = PaddingMode.None;

                des.IV = iv;
                des.Key = key;

                using (var decryptor = des.CreateDecryptor())
                {
                    var plaintext = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                    return plaintext;
                }
            }
        }
    }
}
