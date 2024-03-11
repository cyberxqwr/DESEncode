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

        private int option;
        private byte[] bits;
        private byte[] IVbits;
        private List<byte> bitsList = new List<byte>();
        private List<byte> ivList = new List<byte>();
        private string encWord;
        private string key;
        private int programInput = -1;
        private Util util = new Util();

        public Wspc()
        {

            while (programInput != 0)
            {
                Console.Clear();
                Console.WriteLine(util.options.ElementAt(2));
                Console.WriteLine(util.options.ElementAt(3));
                Console.WriteLine(util.options.ElementAt(4));
                Console.WriteLine(util.options.ElementAt(5));
                int.TryParse(Console.ReadLine(), out programInput);

                switch (programInput)
                {
                    case 1:
                        StartReading(0);
                        StartHashing(1, 0);
                        break;
                    case 2:
                        if (FileReader.FileExists("CBCEnc.txt"))
                        {
                            if (!AskForReading()) StartReading(1);
                            else ReadCertainFile(0);
                        }
                        StartReading(1);
                        StartHashing(2, 1);
                        break;
                    case 3:
                        while (programInput != 9)
                        {
                            Console.Clear();
                            Console.WriteLine(util.options.ElementAt(2));
                            Console.WriteLine(util.optionsLibrary.ElementAt(0));
                            Console.WriteLine(util.optionsLibrary.ElementAt(1));
                            Console.WriteLine(util.optionsLibrary.ElementAt(2));
                            Console.WriteLine(util.optionsLibrary.ElementAt(3));
                            Console.WriteLine(util.optionsLibrary.ElementAt(4));
                            int.TryParse(Console.ReadLine(), out programInput);

                            switch (programInput)
                            {

                                case 0:
                                    return;
                                case 9:
                                    break;
                                case 1:
                                    StartReading(null);
                                    StartLibraryHashing(1);
                                    break;
                                case 2:

                                    if (FileReader.FileExists("CBCEnc.txt"))
                                    {
                                        if (!AskForReading()) StartReading(2);
                                        else ReadCertainFile(1);
                                    }
                                    else StartReading(2);
                                    StartLibraryHashing(2);
                                    break;
                                case 3:
                                    StartReading(null);
                                    StartLibraryHashing(3);
                                    break;
                                case 4:
                                    if (FileReader.FileExists("CFBEnc.txt"))
                                    {
                                        if (!AskForReading()) StartReading(2);
                                        else ReadCertainFile(2);
                                    }
                                    else StartReading(2);
                                    StartLibraryHashing(4);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine(util.exceptions.ElementAt(2));
                        break;
                }

            }

        }

        // 0 - ECB
        // 1 - CBC
        // 2 - CFB

        private void ReadCertainFile(int decryptMode)
        {

            switch (decryptMode)
            {
                case 0:
                    encWord = FileReader.ReadFileToString("ECBEnc.txt");
                    StartReading(3);
                    break;
                case 1:
                    SetData(FileReader.ReadFileToString("CBCEnc.txt"));
                    StartReading(4);
                    break;
                case 2:
                    SetData(FileReader.ReadFileToString("CFBEnc.txt"));
                    StartReading(4);
                    break;
            }
        }

        private void SetData(string plainText)
        {

            StringBuilder sb = new StringBuilder();
            byte byteInput;
            int encPos = 0;

            for (int i = 0; i < plainText.Count(); i++)
            {


                if (plainText[i] != '-')
                {
                    if (plainText[i] != ' ')
                    {
                        sb.Append(plainText[i]);
                    }
                    else
                    {
                        byte.TryParse(sb.ToString(), out byteInput);
                        ivList.Add(byteInput);
                        sb.Clear();
                    }
                }
                else
                {
                    encPos = i + 1;
                    break;
                }
            }

            for (int i = encPos; i < plainText.Count(); i++)
            {

                if (plainText[i] != ' ')
                {
                    sb.Append(plainText[i]);
                }
                else
                {
                    byte.TryParse(sb.ToString(), out byteInput);
                    bitsList.Add(byteInput);
                    sb.Clear();
                }
            }

            IVbits = ivList.ToArray();
            bits = bitsList.ToArray();
        }

        private bool AskForReading()
        {

            Console.WriteLine("Ar norite nuskaityti is failo?");
            Console.WriteLine("[Y] - Taip / [N] - Ne");
            string input;
            input = Console.ReadLine();
            do
            {

                if (input == "Y" || input == "y") return true;

            } while (input != "N" && input != "n");

            return false;
        }

        private string TurnIntoHex(string text)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                if (c != ' ') { sb.Append(Convert.ToByte(c).ToString("x")); }

            }

            return sb.ToString();
        }

        // 0 - ECB (message and key)
        // 1 - ECB (message hex and key)
        // null - CBC and CFB encryption reading
        // 2 - CBC and CFB decrypting (reading bytes and iv)
        // 3 - read from file ECB (only key needed and turning into binary)
        // 4 - read from file CBC/CFB (only key)

        private void StartReading(int? option)
        {

            if (option != 4)
            {
                if (option == 0 || option == 1 || option == null || option == 3)
                {

                    if (option != 3)
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
                    }

                    using (HashTable hashTable = new HashTable())
                    {

                        if (option == 0 || option == null) encWord = hashTable.HexStringToBinary(TurnIntoHex(encWord));
                        else if (option == 1 || option == 3) encWord = hashTable.HexStringToBinary(encWord);
                    }
                }
                else
                {

                    Console.WriteLine(util.options.ElementAt(6));
                    Console.WriteLine(util.options.ElementAt(7));
                    string input = "+";
                    byte byteInput;

                    StringBuilder sb = new StringBuilder();

                    while (input != "-")
                    {

                        input = Console.ReadLine();
                        if (input != "-")
                        {
                            for (int i = 0; i < input.Count(); i++)
                            {
                                if (input[i] != ' ')
                                {
                                    sb.Append(input[i]);
                                }
                                else
                                {
                                    byte.TryParse(sb.ToString(), out byteInput);
                                    bitsList.Add(byteInput);
                                    sb.Clear();
                                }
                            }
                        }
                        else if (input == "-")
                        {
                            break;
                        }
                        else Console.WriteLine("Neteisinga ivestis");
                    }


                    Console.WriteLine(util.info.ElementAt(2));
                    Console.WriteLine(util.info.ElementAt(3));

                    input = "+";

                    sb.Clear();

                    while (input != "-")
                    {

                        input = Console.ReadLine();
                        if (input != "-")
                        {
                            for (int i = 0; i < input.Count(); i++)
                            {
                                if (input[i] != ' ')
                                {
                                    sb.Append(input[i]);
                                }
                                else
                                {
                                    byte.TryParse(sb.ToString(), out byteInput);
                                    ivList.Add(byteInput);
                                    sb.Clear();
                                }
                            }
                        }
                        else if (input == "-")
                        {
                            break;
                        }
                        else Console.WriteLine("Neteisinga ivestis");
                    }

                    Console.WriteLine(util.info.ElementAt(2));

                    IVbits = ivList.ToArray();
                    bits = bitsList.ToArray();
                }
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

            if (key.Length > 64) key = key.Substring(0, 64);

            using (HashTable hashTable = new HashTable())
            {

                key = hashTable.HexStringToBinary(TurnIntoHex(key));
            }

        }

        private void StartHashing(int parameter, int option)
        {

            using (TextHash ENC = new TextHash(encWord, key, option))
            {

                switch (parameter)
                {
                    case 1:
                        Console.WriteLine(util.info.ElementAt(0) + ENC.CryptedMessageValue());
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.WriteLine(util.info.ElementAt(1) + ENC.DecryptedMessageValue());
                        Console.ReadKey();
                        break;
                }

            }

        }

        private void StartLibraryHashing(int parameter)
        {

            switch (parameter)
            {
                case 1:
                    using (LibraryHashing ENC = new LibraryHashing(encWord, key))
                    {
                        Console.WriteLine();
                        ENC.EncryptCbcUser();
                        Console.ReadKey();
                    }
                    break;
                case 2:
                    using (LibraryHashing ENC = new LibraryHashing(bits, IVbits, key))
                    {
                        Console.WriteLine(util.info.ElementAt(1) + ENC.DecryptCbcUser());
                        Console.ReadKey();
                    }
                    break;
                case 3:
                    using (LibraryHashing ENC = new LibraryHashing(encWord, key))
                    {
                        Console.WriteLine();
                        ENC.EncryptCfbUser();
                        Console.ReadKey();
                    }
                    break;
                case 4:
                    using (LibraryHashing ENC = new LibraryHashing(bits, IVbits, key))
                    {
                        Console.WriteLine(util.info.ElementAt(1) + ENC.DecryptCfbUser());
                        Console.ReadKey();
                    }
                    break;

            }

        }

    }
}
