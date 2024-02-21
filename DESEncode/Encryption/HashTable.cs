using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncode.Encryption
{
    public class HashTable
    {
        private readonly int[] hashTable = { 1, 8, 2, 4, 3, 9, 4, 16, 5, 15, 6, 12, 7, 10, 11, 13, 14, 1 };
        public HashTable() { }

        public int[] hashingTable()
        {

            return hashTable;
        }
    }
}
