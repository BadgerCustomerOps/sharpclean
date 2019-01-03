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
            // u = user's character input | t = tab level | n = user's integer choice
            string u = "", t = "";
            int n = -1;
            // good tells us if what the user input was valid or not
            bool good = false;

            for (int i = 0; i < level; i++) t += tab;   // add up tab levels
            Console.WriteLine(t + question);            // write question to console: "[tabs] [question]"

            while (!good)   // loop until a valid input is given
            {
                u = Console.ReadLine();
                if (u == quit) break;   // if u = "q" just return
                else
                {
                    try { n = Convert.ToInt32(u); } // try to convert user's input to an integer
                    catch (InvalidCastException)
                    {
                        Console.WriteLine(command_err + "invalid command\n");
                        continue;
                    }
                    catch (SystemException)
                    {
                        Console.WriteLine(command_err + "invalid command\n");
                        continue;
                    }

                    // no exception caught, everything was valid
                    good = true;
                    cmd = n;
                }
            }
            return good;
        }

        public bool getfile(string question, ref string file, string filetype, int level)
        {
            // u = user's character input | t = tab level
            string u = "", t = "";
            // good tells us if what the user input was valid or not
            bool good = false;

            for (int i = 0; i < level; i++) t += tab; // add up tab levels and print question
            Console.WriteLine(t + question);

            while (!good)
            {
                u = Console.ReadLine();
                if (u == quit) break;
                else if (u.Length > filetype.Length) // the filename must at least be greater than the filetype length
                {
                    if (u.Substring(u.Length - 4) == filetype) // if filetype is good
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
        private readonly string tab = ">> "; // tab characters
        private readonly string quit = "q";  // quit command
    }
}