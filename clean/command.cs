using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sharpclean
{
    class command
    {
        public bool getcmd(string question, ref int cmd, int level)
        {
            string u = "", t = "";
            int n = -1;
            bool good = false;
            for (int i = 0; i < level; i++) t += tab;
            Console.WriteLine(t + question);
            while (!good)
            {
                u = Console.ReadLine();
                if (u == quit) break;
                else
                {
                    try { n = Convert.ToInt32(u); }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine(command_err + "invalid command\n");
                        continue;
                    }
                    catch(SystemException)
                    {
                        Console.WriteLine(command_err + "invalid command\n");
                        continue;
                    }
                    good = true;
                    cmd = n;
                }
            }
            return good;
        }

        public bool getfile(string question, ref string file, string filetype, int level)
        {
            string u = "", t = "";
            bool good = false;
            for (int i = 0; i < level; i++) t += tab;
            Console.WriteLine(t + question);
            while (!good)
            {
                u = Console.ReadLine();
                if (u == quit) break;
                else if (u.Length > filetype.Length)
                {
                    if (u.Substring(u.Length - 4) == filetype)
                        good = true;
                    else
                        Console.WriteLine(tab + command_err + "bad filetype - " + u.Substring(u.Length - 4) + "\n");
                }
                else
                    Console.WriteLine(tab + command_err + "bad file name - " + u + "\n");
            }
            file = u;
            return good;
        }

        private readonly string command_err = "::COMMAND::error : ";
        private readonly string tab = ">> ";
        private readonly string quit = "q";
    }
}