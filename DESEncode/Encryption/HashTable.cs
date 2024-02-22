using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncode.Encryption
{
    public class HashTable : IDisposable
    {
        private readonly int[] hashTable = { 1, 8, 2, 4, 3, 9, 4, 16, 5, 15, 6, 12, 7, 10, 11, 13, 14, 1 };
        private readonly int[] IP =
        {   57, 49, 41, 33, 25, 17, 9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7,
            56, 48, 40, 32, 24, 16, 8, 0,
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6   };

        private readonly int[] PC =
        {   56, 48, 40, 32, 24, 16, 8,
            0, 57, 49, 41, 33, 25, 17,
            9, 1, 58, 50, 42, 34, 26,
            18, 10, 2, 59, 51, 43, 35,
            62, 54, 46, 38, 30, 22, 14,
            6, 61, 53, 45, 37, 29, 21,
            13, 5, 50, 52, 44, 36, 28,
            20, 12, 4, 27, 19, 11, 3    };

        // Disposeable
        bool is_disposed = false;
        public HashTable() { }

        public int[] HashingTable()
        {

            return hashTable;
        }

        public int[] IPTable()
        {

            return IP;
        }

        public int[] PCTable()
        {

            return PC;
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

        public string HexStringToBinary(string hexString)
        {
            StringBuilder bin = new StringBuilder();

            Dictionary<char, string> hexMap = new Dictionary<char, string>{
            { '0', "0000"},
            { '1', "0001"},
            { '2', "0010"},
            { '3', "0011"},
            { '4', "0100"},
            { '5', "0101"},
            { '6', "0110"},
            { '7', "0111"},
            { '8', "1000"},
            { '9', "1001"},

            { 'A', "1010"},
            { 'B', "1011"},
            { 'C', "1100"},
            { 'D', "1101"},
            { 'E', "1110"},
            { 'F', "1111"}};

            foreach (char hexChar in hexString) {

                if (hexMap.TryGetValue(hexChar, out string binString)) bin.Append(binString);

            }

            return bin.ToString();
        }
    }
}
