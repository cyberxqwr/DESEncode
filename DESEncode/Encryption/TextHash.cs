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
        private string keyPC;
        private string hashedWord;
        private int[] IPTable;
        private int[] PCTable;
        
        private List<string> bits64 = new List<string>();
        private List<string> bits64IP = new List<string>();
        private List<string> leftBits32 = new List<string>();
        private List<string> rightBits32 = new List<string>();

        // Disposeable
        bool is_disposed = false;

        public TextHash(string encWord, string key)
        {

            this.encWord = encWord;
            this.key = key;
            using (HashTable hT = new HashTable())
            {
                IPTable = hT.IPTable();
                PCTable = hT.PCTable();
            }
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

            StringBuilder sb = new StringBuilder();

            if (encWord.Length % 64 != 0) {
            
                while (encWord.Length % 64 != 0) {

                    sb.Append("0");
                    encWord += sb.ToString();
                }

                if (encWord.Length % 2 != 0)
                {

                    while (encWord.Length % 2 != 0)
                    {
                        sb.Append("0");
                        encWord += sb.ToString();
                    }
                }
            }

            for (int i = 0; i < encWord.Length; i+=64) bits64.Add(encWord.Substring(i, 64));

            IP();
            Split64Bits();

            sb.Clear();

            if (key.Length % 64 != 0)
            {

                while(key.Length % 64 != 0)
                {

                    sb.Append("0");
                    key += sb.ToString();
                }
            }

            TransformTo56Bits();

        }

        private void IP()
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bits64.Count; i++ )
            {

                for (int j = 0; j < bits64[i].Length; j++) sb.Append(bits64[i].ElementAt(IPTable.ElementAt(j)));

                bits64IP.Add(sb.ToString());

                sb.Clear();
            }
        }

        public int BitCount(string text)
        {
            return Encoding.UTF8.GetByteCount(text) * 8;
        }

        private void StartShuffling()
        {
            char[] newEnc = { };

            StringBuilder sb = new StringBuilder(BitCount(encWord) / 8);
            for (int i = 0; i < BitCount(encWord) / 8; i += 2)
            {
                
            }

            foreach (char c in newEnc) sb.Append(c);

            encWord = sb.ToString();
        }

        private void Split64Bits()
        {

            for (int i = 0; i < bits64IP.Count(); i++)
            {

                leftBits32.Add(bits64IP[i].Substring(0, 32));
                rightBits32.Add(bits64IP[i].Substring(32, 32));
            }
        }

        public string TransformTo56Bits()
        {
            key = "0001001100110100010101110111100110011011101111001101111111110001";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < PCTable.Count(); i++) {

                sb.Append(key[PCTable[i]]);
                if (i % 7 == 6 && i < PCTable.Count()) sb.Append(' ');
            }

            return keyPC = sb.ToString();
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