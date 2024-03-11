using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private string cryptedMessage = "";
        private string decryptedMessage = "";
        private int option;
        private int[] IPTable;
        private int[] FinalIPTable;
        private int[] PCTable;
        private int[] PC2Table;
        private int[] EBIT;

        // 6bit to 4bit refactoring

        private Array[] S;
        int[,] S1;
        int[,] S2;
        int[,] S3;
        int[,] S4;
        int[,] S5;
        int[,] S6;
        int[,] S7;
        int[,] S8;
        private int[] P;

        // MessageBits

        private List<string> bits64 = new List<string>();
        private List<string> bits64IP = new List<string>();
        private List<string> leftBits32 = new List<string>();
        private List<string> rightBits32 = new List<string>();

        // KeyBits

        private List<string> leftBits28 = new List<string>();
        private List<string> rightBits28 = new List<string>();
        private List<string> bits48PC = new List<string>();

        // ExchangeBits

        private List<string> eLeftBits32 = new List<string>();
        private List<string> eRightBits32 = new List<string>();

        // HashedMessage

        private List<string> hashedMessage = new List<string>();

        // Disposeable
        bool is_disposed = false;

        public TextHash(string encWord, string key, int option)
        {

            this.encWord = encWord;
            this.key = key;
            this.option = option;
            using (HashTable hT = new HashTable())
            {
                IPTable = hT.IPTable();
                PCTable = hT.PCTable();
                PC2Table = hT.PC2Table();
                EBIT = hT.EBITs();
                S = hT.STable();
                S1 = (int[,])S[0].Clone();
                S2 = (int[,])S[1].Clone();
                S3 = (int[,])S[2].Clone();
                S4 = (int[,])S[3].Clone();
                S5 = (int[,])S[4].Clone();
                S6 = (int[,])S[5].Clone();
                S7 = (int[,])S[6].Clone();
                S8 = (int[,])S[7].Clone();
                P = hT.PTable();
                FinalIPTable = hT.FinalIPTable();

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

            if (option == 0)
            {

                if (encWord.Length % 64 != 0)
                {

                    while (encWord.Length % 64 != 0)
                    {

                        sb.Append("0");
                        encWord += sb.ToString();
                        sb.Clear();
                    }

                }

                for (int i = 0; i < encWord.Length; i += 64) bits64.Add(encWord.Substring(i, 64));

                IP();

                sb.Clear();

            }
            else
            {

                for (int i = 0; i < encWord.Length; i += 64) bits64.Add(encWord.Substring(i, 64));
                IP();
            }

            if (key.Length / 64 != 1)
            {

                while (key.Length / 64 != 1)
                {

                    sb.Append("0");
                    key += sb.ToString();
                    sb.Clear();
                }
            }

            TransformTo56Bits();
            Split56Bits();
            GenerateSubkeys();

        }

        public void IP()
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bits64.Count(); i++)
            {

                for (int j = 0; j < IPTable.Count(); j++)
                {

                    sb.Append(bits64[i].ElementAt(IPTable.ElementAt(j)));
                }

                bits64IP.Add(sb.ToString());

                sb.Clear();
            }

            Split64Bits();

        }

        private void Split64Bits()
        {

            for (int i = 0; i < bits64IP.Count(); i++)
            {

                leftBits32.Add(bits64IP[i].Substring(0, 32));
                rightBits32.Add(bits64IP[i].Substring(32, 32));
            }
        }

        private void TransformTo56Bits()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < PCTable.Count(); i++)
            {

                sb.Append(key[PCTable[i]]);
            }

            keyPC = sb.ToString();
        }

        private void Split56Bits()
        {

            leftBits28.Add(keyPC.Substring(0, 28));
            rightBits28.Add(keyPC.Substring(28, 28));
        }

        public void GenerateSubkeys()
        {

            int leftShifts;
            int[] singleLS = { 0, 1, 8, 15 };

            using (Shifter sf = new Shifter())
            {

                for (int i = 0; i < 16; i++)
                {

                    if (singleLS.Contains(i))
                    {
                        leftShifts = 1;
                    }
                    else leftShifts = 2;

                    leftBits28.Add(sf.ShiftLeft(leftShifts, leftBits28.ElementAt(i)));
                    rightBits28.Add(sf.ShiftLeft(leftShifts, rightBits28.ElementAt(i)));

                }

            }

            PC2();

        }

        private void PC2()
        {

            StringBuilder sb = new StringBuilder();
            string listSum;

            for (int i = 0; i < 16; i++)
            {

                listSum = leftBits28[i + 1] + rightBits28[i + 1];

                for (int j = 0; j < PC2Table.Count(); j++)
                {

                    sb.Append(listSum.ElementAt(PC2Table[j]));
                }

                bits48PC.Add(sb.ToString());
                sb.Clear();
            }

            if (option == 0) ExchangeHandle();
            else DecryptExchangeHandle();

        }

        private void ExchangeHandle()
        {

            string binaryString;
            string reformedBits;

            for (int i = 0; i < leftBits32.Count; i++)
            {

                eLeftBits32.Clear();
                eRightBits32.Clear();

                eLeftBits32.Add(rightBits32[i]);
                binaryString = XorFunction(rightBits32[i], bits48PC[0]);
                reformedBits = PermutateXoredBinary(Reform6BitBlocks(binaryString));
                eRightBits32.Add(XorBeforeFP(leftBits32[i], reformedBits));

                for (int j = 0; j < 15; j++)
                {

                    eLeftBits32.Add(eRightBits32[j]);
                    binaryString = XorFunction(eRightBits32[j], bits48PC[j + 1]);
                    reformedBits = PermutateXoredBinary(Reform6BitBlocks(binaryString));
                    eRightBits32.Add(XorBeforeFP(eLeftBits32[j], reformedBits));
                }

                FinalPermutation();
            }
        }

        private void FinalPermutation()
        {

            hashedWord = eRightBits32.Last() + eLeftBits32.Last();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < FinalIPTable.Count(); i++)
            {

                sb.Append(hashedWord.ElementAt(FinalIPTable[i]));
            }

            hashedWord = sb.ToString();
            GetEncryptedMessage();
        }

        private void GetEncryptedMessage()
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashedWord.Count(); i++)
            {

                sb.Append(hashedWord[i]);
                if (i % 4 == 3)
                {

                    hashedMessage.Add(sb.ToString());
                    sb.Clear();
                }
            }

            using (HashTable hashTable = new HashTable())
            {
                if (option == 0) cryptedMessage += hashTable.BinaryToHexString(hashedMessage) + " ";
                else decryptedMessage += hashTable.HexToString(hashTable.BinaryToHexString(hashedMessage)) + " ";

                hashedMessage.Clear();
            }
        }

        private string XorBeforeFP(string leftBits, string reformedBits)
        {

            StringBuilder sb = new StringBuilder();


            for (int i = 0; i < leftBits.Count(); i++)
            {

                if (leftBits[i] == '0' && reformedBits[i] == '1' || leftBits[i] == '1' && reformedBits[i] == '0') sb.Append("1");
                else sb.Append("0");
            }

            return sb.ToString();
        }

        private string PermutateXoredBinary(string reformedBits)
        {

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < reformedBits.Count(); i++)
            {

                sb.Append(reformedBits.ElementAt(P[i]));
            }

            return sb.ToString();
        }

        private string XorFunction(string rightBits, string keyPC)
        {

            string reformed;

            StringBuilder sb = new StringBuilder();
            rightBits = ExpandRightBits(rightBits);

            string[] binaries = { "", "", "" };
            string binaryString = "";
            int[] decimals = { 0, 0, 0 };

            for (int i = 0; i < keyPC.Count(); i += 6)
            {

                binaries[0] = sb.Append(keyPC.Substring(i, 6)).ToString();
                sb.Clear();
                binaries[1] = sb.Append(rightBits.Substring(i, 6)).ToString();
                sb.Clear();
                decimals[0] = Convert.ToInt32(binaries[0], 2);
                decimals[1] = Convert.ToInt32(binaries[1], 2);
                decimals[2] = decimals[0] ^ decimals[1];
                binaries[2] = Convert.ToString(decimals[2], 2).PadLeft(6, '0');
                binaryString += binaries[2] + " ";
            }

            return binaryString;
        }

        private string ExpandRightBits(string rightBits)
        {


            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < EBIT.Count(); i++)
            {

                sb.Append(rightBits.ElementAt(EBIT[i]));
            }

            return sb.ToString();
        }

        private string Reform6BitBlocks(string binaryString)
        {

            string reformedBits = "";
            List<string> temp6Bits = new List<string>();
            int row, col;
            int[] middleChars = { 1, 2, 3, 4 };


            StringBuilder sb = new StringBuilder();

            foreach (char c in binaryString)
            {
                if (c != ' ')
                {
                    sb.Append(c);
                }
                else
                {
                    temp6Bits.Add(sb.ToString());
                    sb.Clear();
                }

            }

            sb.Clear();

            for (int i = 0; i < temp6Bits.Count; i++)
            {

                // Find row

                sb.Append(temp6Bits[i].First());
                sb.Append(temp6Bits[i].Last()).ToString();
                row = Convert.ToInt32(sb.ToString(), 2);
                sb.Clear();

                // Find column

                foreach (int j in middleChars) sb.Append(temp6Bits[i].ElementAt(j));
                col = Convert.ToInt32(sb.ToString(), 2);
                sb.Clear();

                switch (i)
                {
                    case 0:

                        temp6Bits[i] = Convert.ToString(S1[row, col], 2).PadLeft(4, '0');
                        break;
                    case 1:
                        temp6Bits[i] = Convert.ToString(S2[row, col], 2).PadLeft(4, '0');
                        break;
                    case 2:
                        temp6Bits[i] = Convert.ToString(S3[row, col], 2).PadLeft(4, '0');
                        break;
                    case 3:
                        temp6Bits[i] = Convert.ToString(S4[row, col], 2).PadLeft(4, '0');
                        break;
                    case 4:
                        temp6Bits[i] = Convert.ToString(S5[row, col], 2).PadLeft(4, '0');
                        break;
                    case 5:
                        temp6Bits[i] = Convert.ToString(S6[row, col], 2).PadLeft(4, '0');
                        break;
                    case 6:
                        temp6Bits[i] = Convert.ToString(S7[row, col], 2).PadLeft(4, '0');
                        break;
                    case 7:
                        temp6Bits[i] = Convert.ToString(S8[row, col], 2).PadLeft(4, '0');
                        break;
                    default: break;
                }

                reformedBits += temp6Bits[i];
            }

            return reformedBits;

        }

        public string CryptedMessageValue()
        {
            Console.WriteLine("Ar norite irasyti i faila?");
            Console.WriteLine("[Y] - Taip / [N] - Ne");
            string input;
            input = Console.ReadLine();
            do
            {

                if (input == "Y" || input == "y")
                {

                    FileReader.WriteEncryptedMessageToFile("ECBEnc.txt", cryptedMessage);
                    Console.WriteLine("\n\nFailas irasytas\n\n");
                    break;
                }

            } while (input != "N" && input != "n");

            return cryptedMessage;
        }

        private void DecryptExchangeHandle()
        {

            string binaryString;
            string reformedBits;

            for (int i = 0; i < leftBits32.Count; i++)
            {

                eLeftBits32.Clear();
                eRightBits32.Clear();

                eLeftBits32.Add(rightBits32[i]);
                binaryString = XorFunction(rightBits32[i], bits48PC[15]);
                reformedBits = PermutateXoredBinary(Reform6BitBlocks(binaryString));
                eRightBits32.Add(XorBeforeFP(leftBits32[i], reformedBits));

                for (int j = 0; j < 15; j++)
                {

                    eLeftBits32.Add(eRightBits32[j]);
                    binaryString = XorFunction(eRightBits32[j], bits48PC[14 - j]);
                    reformedBits = PermutateXoredBinary(Reform6BitBlocks(binaryString));
                    eRightBits32.Add(XorBeforeFP(eLeftBits32[j], reformedBits));
                }

                FinalPermutation();
            }

        }

        public string DecryptedMessageValue()
        {

            return decryptedMessage;
        }

    }
}