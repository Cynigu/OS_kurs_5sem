using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_kurs
{
    public class MyClaster
    {
        private int index;
        private string claster;

        public int Index { get => index; set => index = value; }
        public string Claster { get => claster; set => claster = value; }

        public MyClaster() 
        {
        }

        public MyClaster(int inx, string str)
        {
            int t;
            if (inx == 0 && str != "-")
            {
                throw new ArgumentException();
            }
            else if (inx!=0 && str != "" && str != "eof"
                && str != "bad" && !int.TryParse(str, out t))
            {
                throw new ArgumentException();
            }
            Index = inx;
            Claster = str;
        }
    }
}
