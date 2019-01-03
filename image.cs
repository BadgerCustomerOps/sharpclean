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
            // try to open file
            System.IO.StreamReader infile = null;
            try {
                infile = new System.IO.StreamReader(filename, Encoding.UTF7);
            }
            catch (System.IO.FileNotFoundException) {
                Console.WriteLine(image_err + "could not open file: " + filename + "\n");
                return false;
            }

            // first line is always version
            mdata.filetype = infile.ReadLine().Substring(0, 2);
            if (!(mdata.filetype != "P5" || mdata.filetype != "P2")) {
                Console.WriteLine(image_err + "invalid file type: " + mdata.filetype + "\n");
                return false;
            }

            // ignore comments
            string comments = "";
            while (true) {
                comments = infile.ReadLine();
                if (comments[0] != '#') break;
            }

            // get width, height, and total
            string[] ss = comments.Split();
            mdata.width = int.Parse(ss[0]);
            mdata.height = int.Parse(ss[1]);
            mdata.totalpixels = mdata.width * mdata.height;
            
            // get maximum grey value in file
            mdata.maxgreyval = Convert.ToInt16(infile.ReadLine());

            // get image data
            if (mdata.filetype == "P2") loadP2(infile);
            else loadP5(infile);

            Console.WriteLine("successfully loaded...\n");
            dataLoaded = true;

            infile.Close();

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
                
                // write header info
                p2write.Write(mdata.filetype + "\n" + "# Created by Sharp Clean Software\n" + mdata.width + " " + mdata.height + "\n" + mdata.maxgreyval + "\n");

                // write all of the pixel data one line at a time
                for (int i = 0; i < mdata.totalpixels; i++)
                    p2write.WriteLine(Convert.ToString(pixels[i].value));

                // close stream
                p2write.Close();
            }
            else
            {
                StreamWriter p5write = new StreamWriter(filename, false, Encoding.Default); // notice: encoding must be set to default (ansi)

                // write header info
                p5write.Write(mdata.filetype + "\n# Created by Sharp Clean Software\n" + mdata.width + " " + mdata.height + "\n" + mdata.maxgreyval + "\n");

                // write pixel data on one line
                for (int i = 0; i < mdata.totalpixels; i++)
                    p5write.Write(Convert.ToChar(pixels[i].value));

                // flush and close - i think close() flushes already but whatever
                p5write.Flush();
                p5write.Close();
            }

        }
        private void loadP2(System.IO.StreamReader f)
        {
            // initialize pixel with total pixel size
            pixels = new pixel[mdata.totalpixels];
            string line;
            int i = 0; // i will be the pixel id: [0 -> total pixels]

            while ((line = f.ReadLine()) != null) // loop over each line
            {
                pixels[i].value = Convert.ToByte(line); // convert the line to a byte (unsigned)
                pixels[i].id = i; // set id and found/selected bools -- these are used in the cleaning algorithms
                pixels[i].found = false;
                pixels[i].selected = false;
                i++;
            }
        }

        private void loadP5(System.IO.StreamReader f)
        {
            // set size of pixels, create a character array buffer
            pixels = new pixel[mdata.totalpixels];
            char[] buffer = new char[mdata.totalpixels];

            // read the entire pixel data block into the character array
            f.ReadBlock(buffer, 0, mdata.totalpixels);

            for (int i = 0; i < mdata.totalpixels; i++) // for each pixel
            {
                pixels[i].value = Convert.ToByte(buffer[i]); // convert character to byte
                pixels[i].id = i; // set id and found/selected bools -- these are used in the cleaning algorithms
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
                    case 1: print(); break; // print every single pixel loaded as it's integer representation - warning: this could produce thousands of lines
                    case 2: printChoice(); break;
                }
            }
        }

        private void print() // print every single pixel loaded as it's integer representation - warning: this could produce thousands of lines
        {
            for (int i = 0; i < mdata.totalpixels; i++)
                Console.WriteLine((Convert.ToChar(pixels[i].value) + " ").PadRight(4));
        }

        private void printChoice() // an extra level of print options
        {
            int n = 0;
            if (cmd.getcmd("[1]all [2]file type, [3]dimensions, [4]total pixels, [5]max grey value - ", ref n, 2))
                print(n - 1);
        }

        private void print(int i)
        {
            switch (i)
            {
                case 0: // print all info by recursively calling print(int)
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
        private data mdata;     // header data struct
        private pixel[] pixels; // pixel data array
        private command cmd = new command();
        private readonly string image_err = "::IMAGE::error : ";
    }
}