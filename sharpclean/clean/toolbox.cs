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

        public toolbox(pixel[] p, int width, int height, int total)
        {
            pixels = p;
            imageWidth = width;
            totalPixels = total;
            imageHeight = height;
        }

        //gets some info for saving data, then taps run()
        public void clean(ProgressBar progressBar1)
        {
            if (pixels == null)
            {
                MessageBox.Show("No Pixels Loaded", "no pixels", 0);
                return;
            }
            run(progressBar1);
        }

        //the big boy, iterates through the pixels and drives algorithms
        private void run(ProgressBar progressBar1)
        {
            // Timing the runtime
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            selection s = new selection(pixels, imageWidth, totalPixels);
            watch.Start();
            for (int i = 0; i < totalPixels; i++)
            {
                // Testing
                progressBar1.Value = i;

                if (s.get(i))
                {
                    buffer = s.Buffer;
                    perimeter = s.Perimeter;
                    objectData dat = new objectData(getAverageValue(buffer.Count), buffer.Count, buffer.Count / s.getEdges());
                    conf c = confidence.getconfidence(dat);
                    dat.objconf = c;
                    objdat.Add(dat);

                    if (!c.isObj)
                        colorbuffer(COLOR_CLEAR, buffer.Count);

                }
                s.clearBuffer();
                buffer.Clear();
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

        // debugging funciton to look at the walk path
        public void PrintWalkPath(fileOps mapCleanup)
        {
            for (int i = 0; i < mapCleanup.walkPath.Count(); ++i)
            {
                Console.WriteLine("x: " +  mapCleanup.walkPath[i].x.ToString() + " y: " + mapCleanup.walkPath[i].y.ToString());
            }
        }

        // removes the path that was walked by using the the walk path genertated from the trajectory file in fileops
        public void removeDebris(fileOps mapCleanup)
        {
            for (int x = 0; x < mapCleanup.walkPath.Count(); ++x)
            {
                dualtosingular(mapCleanup.walkPath[x].x, mapCleanup.walkPath[x].y);
            }
        }

        // converts the 2d xy to 1d location
        private void dualtosingular(int x, int y)
        {
            int width = imageWidth;
            int height = imageHeight;
            y = height - y;
            int trajectoryLocation = ((y - 1) * width) + x;
            Brush(trajectoryLocation);
        }
        // changes the color of the pixels and sets the touch value
        private void Brush(int trajectoryLocation)
        {
            int color = 0;
            for (int i = -7; i < 8; i++)
            {
                for (int k = -7; k < 8; k++)
                {
                    int pixelLocation = trajectoryLocation + ((imageWidth * k) + i);
                    pixels[pixelLocation].value = Convert.ToByte(color);
                    pixels[pixelLocation].selected = true;
                }
            }
        }
        private double getAverageValue(int sizeofbuffer)
        {
            double avg = 0;
            for (int i = 0; i < sizeofbuffer; i++)
                avg += pixels[buffer[i]].value;
            return avg / sizeofbuffer;
        }

        public List<objectData> getObjectData()
        {
            return objdat;
        }

        private pixel[] pixels = null;
        private command cmd = new command();
        private int imageWidth, totalPixels, imageHeight;
        private List<int> buffer = new List<int>();
        private List<int> perimeter = new List<int>();
        private List<objectData> objdat = new List<objectData>();
    }
}