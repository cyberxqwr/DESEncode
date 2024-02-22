using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DESEncode.Encryption;
using DESEncode.Utilities;

namespace DESEncode.Workspace
{
    public class Wspc
    {

        private Regex par = new Regex("[^a-zA-Z0-9]");
        private string encWord;
        private string key;
        private int programInput = -1;
        private Util util = new Util();

        public Wspc()
        {

            while (programInput != 0)
            {

                Console.WriteLine(util.options.ElementAt(2));
                Console.WriteLine(util.options.ElementAt(3));
                Console.WriteLine(util.options.ElementAt(4));
                Console.WriteLine(util.options.ElementAt(5));
                Console.WriteLine(util.options.ElementAt(6));
                int.TryParse(Console.ReadLine(), out programInput);

                switch (programInput)
                {
                    case 1:
                        StartReading();
                        StartHashing(1);
                        break;
                    case 2:
                        StartReading();
                        StartHashing(2);
                        break;
                    case 3:
                        StartReading();
                        StartHashing(3);
                        break;
                    case 4:
                        StartReading();
                        StartHashing(4);
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine(util.exceptions.ElementAt(2));
                        break;
                }

            }

        }

        private string TurnIntoHex(string text)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                if (c != ' ') { sb.Append(Convert.ToByte(c).ToString("x")); }
                else { sb.Append(c); }

            }

            return sb.ToString();
        }

        private void StartReading()
        {

            Console.WriteLine(util.options.ElementAt(0));
            encWord = Console.ReadLine();

            if (encWord.Length == 0)
            {

                while (encWord.Length == 0)
                {

                    Console.WriteLine(util.exceptions.ElementAt(0));
                    encWord = Console.ReadLine();
                }

            }

            encWord = TurnIntoHex(encWord);

            using (HashTable hashTable = new HashTable())
            {

                encWord = hashTable.HexStringToBinary(TurnIntoHex(encWord));
            }

            Console.WriteLine(util.options.ElementAt(1));
            key = Console.ReadLine();

            if (key.Length == 0)
            {

                while (key.Length == 0)
                {

                    Console.WriteLine(util.exceptions.ElementAt(1));
                    key = Console.ReadLine();
                }
            }

            using (HashTable hashTable = new HashTable())
            {

                key = hashTable.HexStringToBinary(TurnIntoHex(key));
            }

        }

        private void StartHashing(int parameter)
        {

            using (TextHash ENC = new TextHash(encWord, key))
            {

                switch (parameter)
                {
                    case 1:
                        Console.WriteLine(util.info.ElementAt(0) + ENC.StartHashing());
                        break;
                    case 2:

                        break;
                    case 3:
                        Console.WriteLine(util.info.ElementAt(1) + ENC.StartUnhashing());
                        break;
                    case 4:

                        break;
                }

            }


        }

        private string CheckForViolations(string text)
        {

            if (!par.IsMatch(text))
            {
                return text;
            }
            else
            {
                do
                {
                    Console.WriteLine(util.exceptions.ElementAt(3));
                    text = Console.ReadLine();
                } while (par.IsMatch(text));
            }

            return text; ;

        }

    }
}
