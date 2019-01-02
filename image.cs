using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sharpclean
{
    class image
    {
        public bool load(string filename)
        {
            System.IO.StreamReader infile = null;
            try
            {
                infile = new System.IO.StreamReader(filename, Encoding.UTF7);
            }

            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine(image_err + "could not open file: " + filename + "\n");
                return false;
            }
            //first line is always version
            mdata.filetype = infile.ReadLine().Substring(0, 2);
            Console.WriteLine(mdata.filetype);
            if (!(mdata.filetype != "P5" || mdata.filetype != "P2"))
            {
                Console.WriteLine(image_err + "invalid file type: " + mdata.filetype + "\n");
                return false;
            }
            //ignore comments
            string comments = "";
            while (true)
            {
                comments = infile.ReadLine();
                Console.WriteLine(comments);
                if (comments[0] != '#') break;
            }
            //get width, height, and total
            string[] ss = comments.Split();
            Console.WriteLine(ss[0] + "," + ss[1]);
            mdata.width = int.Parse(ss[0]);
            mdata.height = int.Parse(ss[1]);
            mdata.totalpixels = mdata.width * mdata.height;
            Console.WriteLine(mdata.totalpixels);
            
            //get maximum grey value in file
            mdata.maxgreyval = Convert.ToInt16(infile.ReadLine());
            Console.WriteLine(mdata.maxgreyval);

            //get image data
            if (mdata.filetype == "P2") loadP2(infile);
            else loadP5(infile);

            Console.WriteLine("successfully loaded...\n");
            dataLoaded = true;
            return true;
        }

        public void write(string filename)
        {
            if (!dataLoaded) {
                Console.WriteLine(image_err + "image data not loaded\n");
                return;
            }

            if (mdata.filetype == "P2")
            {
                StreamWriter p2write = new StreamWriter(filename, false);

                Console.WriteLine("P2 Writing");
                p2write.Write(mdata.filetype + "\n" + "# Created by Sharp Clean Software\n" + mdata.width + " " + mdata.height + "\n" + mdata.maxgreyval + "\n");

                for (int i = 0; i < mdata.totalpixels; i++)
                    p2write.WriteLine(Convert.ToString(pixels[i].value));
            }
            else
            {
                StreamWriter p5write = new StreamWriter(filename, false, Encoding.Default);

                Console.WriteLine("P5 Writing");
                p5write.Write(mdata.filetype + "\n# Created by Sharp Clean Software\n" + mdata.width + " " + mdata.height + "\n" + mdata.maxgreyval + "\n");

                for (int i = 0; i < mdata.totalpixels; i++)
                    p5write.Write(Convert.ToChar(pixels[i].value));

                p5write.Flush();
            }

        }
        private void loadP2(System.IO.StreamReader f)
        {
            pixels = new pixel[mdata.totalpixels];
            string line;
            int i = 0;

            while ((line = f.ReadLine()) != null)
            {
                pixels[i].value = Convert.ToByte(line);
                pixels[i].id = i;
                pixels[i].found = false;
                pixels[i].selected = false;
                i++;
            }
        }

        private void loadP5(System.IO.StreamReader f)
        {
            pixels = new pixel[mdata.totalpixels];
            char[] buffer = new char[mdata.totalpixels];

            f.ReadBlock(buffer, 0, mdata.totalpixels);

            for (int i = 0; i < mdata.totalpixels; i++)
            {
                pixels[i].value = Convert.ToByte(buffer[i]);
                pixels[i].id = i;
                pixels[i].selected = false;
                pixels[i].found = false;
            }

            
        }

        public void printmenu()
        {
            if (!dataLoaded) {
                Console.WriteLine(image_err + "image data not loaded\n");
                return;
            }

            int n = 0;
            if (cmd.getcmd("[1]print all, [2]image data menu, [q]quit - ", ref n, 1))
            {
                switch (n) {
                    case 1: print(); break;
                    case 2: printChoice(); break;
                }
            }
        }

        private void print()
        {
            for (int i = 0; i < mdata.totalpixels; i++)
                Console.WriteLine((Convert.ToChar(pixels[i].value) + " ").PadRight(4));
        }

        private void printChoice()
        {
            int n = 0;
            if (cmd.getcmd("[1]all [2]file type, [3]dimensions, [4]total pixels, [5]max grey value - ", ref n, 2))
                print(n - 1);
        }

        private void print(int i)
        {
            switch (i)
            {
                case 0:
                    {
                        print(Convert.ToInt16(info.FILETYPE));
                        print(Convert.ToInt16(info.DIMENSIONS));
                        print(Convert.ToInt16(info.TOTALPIXELS));
                        print(Convert.ToInt16(info.MAXGREYVAL));
                        break;
                    }
                case 1: Console.WriteLine("file type: " + mdata.filetype + "\n"); break;
                case 2: Console.WriteLine("width: %i, height: %i\n", mdata.width, mdata.height); break;
                case 3: Console.WriteLine("total pixels: %i\n", mdata.totalpixels); break;
                case 4: Console.WriteLine("max grey value: %i\n", mdata.maxgreyval); break;
            }
        }

        public pixel[] getpixels()
        {
            return pixels;
        }

        public ref data getImageData()
        {
            return ref mdata;
        }

        public bool getDataLoaded()
        {
            return dataLoaded;
        }

        private bool dataLoaded = false;
        private data mdata;
        private pixel[] pixels;
        private command cmd = new command();
        private readonly string image_err = "::IMAGE::error : ";
    }
}
