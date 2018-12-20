﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sharpclean
{
    class main
    {
        static int Main(string[] args)
        {
            home m_h = new home();

            return 0;
        }
    }

    class home
    {
        //enter here
        public void run()
        {
            while (good)
            {
                if (!img.getDataLoaded())
                    startmenu();
                else
                    fullmenu();
            }

            deleteimage();
        }

        //initial menu when no image is loaded
        private void startmenu()
        {
            int n = 0;
            if (cmd.getcmd("[1]load - ", ref n, 0))
            {
                if (n == 1)
                    loadfile();
                else
                    System.Console.WriteLine(home_err + "invalid command\n");
            }
            else
                good = false;
        }

        //menu with all options, only when image is loaded
        private void fullmenu()
        {
            int n = 0;
            if (cmd.getcmd("[1]load, [2]save, [3]print menu, [4]clean [q]quit - ", ref n, 0))
            {
                switch (n)
                {
                    case 1: loadfile(); break;
                    case 2: writefile(); break;
                    case 3:
                        {
                            if (!img.getDataLoaded())
                                img.printmenu();
                            else
                                System.Console.WriteLine(home_err + "image not loaded\n");
                            break;
                        }
                    case 4:
                        {
                            /*
                            if (t != NULL)
                                t->clean();
                            else
                                System.Console.WriteLine(home_err + "toolbox not loaded\n");
                            break;
                            */
                            break;
                        }
                }
            }
            else good = false;
        }

        //loads a pgm
        private void loadfile()
        {
            string f = "";
            int n = 0;

            if (!img.getDataLoaded()) {
                if (cmd.getcmd("delete loaded image? [1]yes, [q]quit - ", ref n, 1))
                    deleteimage();
                else return;
            }

            if (cmd.getfile("enter file name : ", ref f, ".pgm", 1)) {
                f = "images/" + f;

                /*if (img.load(f))
                    t = new Toolbox(img->getpixels(), img->getImageData().width, img->getImageData().totalpixels);
                else 
                    deleteimage();
                */
            }
        }

        //writes to a pgm
        private void writefile()
        {
            if (img.getDataLoaded())
            {
                string f = "";
                if (cmd.getfile("enter file name : ", ref f, ".pgm", 1))
                {
                    f = "saves/" + f;
                    img.write(f);
                }
            }
            else System.Console.WriteLine(home_err + "no image loaded to save");
        }

        //deletes the loaded image (only within the program)
        private void deleteimage()
        {
            /*
            if (t != NULL)
            {
                delete t;
                t = NULL;
            }
            delete img;
            img = NULL;
            */
        }

        private bool good = true;
        private command cmd;
        //Toolbox* t;
        image img = new image();

        private readonly string home_err = "::HOME::error : ";

    }
}
