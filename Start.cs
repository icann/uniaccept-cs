using System;
using System.Collections.Generic;
using System.Text;
using org.icann.tld;

namespace ICANN_CS
{
    class Start
    {
        static void Main(string[] args)
        {
            Main main = new Main();
            main.main(args);
            string[] rl = new string[1];
            rl[0] = Console.ReadLine();
            if (rl[0].Trim().ToUpper().Equals("TEST"))
            {
                main.test();
                Console.Read();
            }
        }
    }
}
