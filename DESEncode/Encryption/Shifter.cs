using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncode.Encryption
{
    public class Shifter : IDisposable
    {

        bool is_disposed = false;
        private string temp;

        public Shifter() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.is_disposed = true;
        }

        public string ShiftLeft(int shiftBy, string key) {

            StringBuilder sb = new StringBuilder();

            for (int i = shiftBy; i < key.Count(); i++)
            {

                sb.Append(key[i]);
            }

            switch (shiftBy)
            {
                case 1:
                    sb.Append(key[0]);
                    break;
                case 2:
                    sb.Append(key[0]);
                    sb.Append(key[1]);
                    break;
                    default:
                    break;
            }

            return temp = sb.ToString();
        }
    }
}
