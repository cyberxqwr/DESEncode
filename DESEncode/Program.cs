using DESEncode.Encryption;
using DESEncode.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncode
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //Wspc wspc = new Wspc();
            //StringBuilder sb = new StringBuilder();
            //string text = "Your lips are smoother than vaseline";
            //foreach (char c in text)
            //{
            //    if (c != ' ') { sb.Append(Convert.ToByte(c).ToString("x")); }
            //    else { sb.Append(c); }

            //}

            //HashTable hashTable = new HashTable();
            //string binValues = hashTable.HexStringToBinary("0123456789ABCDEF");
            //Console.WriteLine(sb.ToString());
            //Console.WriteLine(binValues);
            //string key = "abcdef2";
            //int bitCount = Encoding.UTF8.GetByteCount(key) * 8;

            //Console.WriteLine($"Text takes {bitCount} bits in UTF-8.");

            TextHash tH = new TextHash("", "");
            Console.WriteLine(tH.TransformTo56Bits());
        }
    }
}
