using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sharpclean
{
    class toolbox
    {
        public readonly int COLOR_CLEAR = 255;

        public toolbox(pixel[] p, int width, int total)
        {
            pixels = p;
            imageWidth = width;
            totalPixels = total;
        }

        //gets some info for saving data, then taps run()
        public void clean()
        {
            if (pixels == null)
            {
                MessageBox.Show("No Pixels Loaded", "no pixels", 0);
                return;
            }
            /*
            int n = 0;
            ofilename = "none";
            if (cmd.getcmd("write data to .csv file? [1]yes, [2]no, [q]quit - ", ref n, 2))
            {
                if (n == 1)
                    cmd.getfile("enter data output file name : ", ref ofilename, ".csv", 2);
                run();
            }
            */
            run();
        }

        //the big boy, iterates through the pixels and drives algorithms
        private void run()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            Console.WriteLine(totalPixels);
            int per_25 = totalPixels / 4;
            int per_50 = totalPixels / 2;
            int per_75 = per_25 + per_50;
            bool b_25 = false, b_50 = false, b_75 = false;
            /*
            bool writeData = false;
            if (ofilename != "none")
            {
                ofilename = "data/" + ofilename;
                System.IO.File.WriteAllText(ofilename, "val, size, edge, dust, obj, res, type, c avg, c edge, c size\n");
                writeData = true;
            }
            */
            selection s = new selection(pixels, imageWidth, totalPixels);
            watch.Start();
            for (int i = 0; i < totalPixels; i++)
            {
                if (s.get(i))
                {
                    buffer = s.Buffer;
                    perimeter = s.Perimeter;
                    data[1] = buffer.Count;
                    data[2] = data[1] / s.getEdges();
                    data[0] = getAverageValue(Convert.ToInt32(data[1]));

                    conf c = confidence.getconfidence(data);

                    if (!c.isObj)
                        colorbuffer(COLOR_CLEAR, Convert.ToInt32(data[1]));

                    //if (writeData)
                    //    printcsv(ref c);
                }
                s.clearBuffer();
                buffer.Clear();
                if (i > per_25 && !b_25)
                {
                    Console.WriteLine("25%...\n");
                    b_25 = true;
                }

                if (i > per_50 && !b_50)
                {
                    Console.WriteLine("50%...\n");
                    b_50 = true;
                }

                if (i > per_75 && !b_75)
                {
                    Console.WriteLine("75%...\n");
                    b_75 = true;
                }
            }
            watch.Stop();
            Console.WriteLine("Time elapsed: {0}", watch.Elapsed);
        }

        //colors a selection of pixels
        private void colorbuffer(int color, int sizeofbuffer)
        {
            for (int i = 0; i < sizeofbuffer; i++)
                pixels[buffer[i]].value = Convert.ToByte(color);
        }

        //colors the edges of a selection of pixels
        private void coloredges(int color, int sizeofperimeter)
        {
            for (int i = 0; i < sizeofperimeter; i++)
                pixels[perimeter[i]].value = Convert.ToByte(color);
        }

        //writes some data to a csv, if the user wants
        /*
        private void printcsv(ref conf c)
        {
            System.IO.File.WriteAllText(ofilename, data[0] + "," + data[1] + "," + data[2] + "," + c.dust + "," + c.obj + ",");
            if (c.isObj)
                System.IO.File.WriteAllText(ofilename, (c.obj - c.dust) + ",obj," + (c.o_val - c.d_val) + "," + (c.o_edge - c.d_edge) + "," + (c.o_size - c.d_size) + "\n");
            else
                System.IO.File.WriteAllText(ofilename, (c.dust - c.obj) + ",dust," + (c.d_val - c.o_val) + "," + (c.d_edge - c.o_edge) + "," + (c.d_size - c.o_size) + "\n");
        }
        */
        //gets some data on the selection
        private double getAverageValue(int sizeofbuffer)
        {
            double avg = 0;
            for (int i = 0; i < sizeofbuffer; i++)
                avg += pixels[buffer[i]].value;
            return avg / sizeofbuffer;
        }

        private pixel[] pixels = null;
        private command cmd = new command();
        private int imageWidth, totalPixels;
        private List<int> buffer = new List<int>();
        private List<int> perimeter = new List<int>();
        private double[] data = new double[3]; //average value, size, number of edges
        //private string ofilename;
        //private readonly string toolbox_err = "::TOOLBOX::error : ";
    }
}