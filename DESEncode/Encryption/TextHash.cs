using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESEncode.Encryption;

namespace DESEncode.Encryption
{
    public class TextHash : IDisposable
    {

        private string encWord;
        private string key;
        private string hashedWord;
        private int[] hashTable;


        // Disposeable
        bool is_disposed = false;

        public TextHash(string encWord, string key)
        {

            this.encWord = encWord;
            this.key = key;
            HashTable hT = new HashTable();
            hashTable = hT.hashingTable();
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

        private void RegulateEncryption()
        {

            int keyBits = bitCount(key);

            if (keyBits < 56)
            {

                StringBuilder sbk = new StringBuilder((56 / 8) - (keyBits / 8));

                while (keyBits != 56)
                {

                    sbk.Append("0");
                    keyBits += 8;
                }

                key = key + sbk.ToString();
            }
            else key = key.Substring(0, 7);

            int encBits = bitCount(encWord);

            if (encBits < 128)
            {

                StringBuilder sbk = new StringBuilder((128 / 8) - (encBits / 8));

                while (encBits != 128)
                {

                    sbk.Append("0");
                    encBits += 8;
                }

                encWord = encWord + sbk.ToString();
            }
            else encWord = encWord.Substring(0, 16);

        }

        public int bitCount(string text)
        {
            return Encoding.UTF8.GetByteCount(text) * 8;
        }

        private void startShuffling()
        {
            char[] newEnc = { };

            StringBuilder sb = new StringBuilder(bitCount(encWord) / 8);
            for (int i = 0; i < bitCount(encWord) / 8; i += 2)
            {
                newEnc[hashTable[i]] = encWord[hashTable[i]];
                newEnc[hashTable[i + 1]] = encWord[hashTable[i + 1]];
            }

            foreach (char c in newEnc) sb.Append(c);

            encWord = sb.ToString();
        }






























        public string StartHashing()
        {

            HashText();
            return hashedWord;
        }

        private void HashText()
        {

            StringBuilder sb = new StringBuilder();

            hashedWord = sb.ToString();
        }

        public string StartUnhashing()
        {
            UnhashText();
            return hashedWord;
        }

        private void UnhashText()
        {

            StringBuilder sb = new StringBuilder();

            hashedWord = sb.ToString();
        }

    }
}