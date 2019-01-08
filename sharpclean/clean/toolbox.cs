using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * Sharp Clean: clean/toolbox.cs
 * Author: Austin Herman
 * Edited: Jair Ramirez 1/4/2017
 */

namespace sharpclean
{
    class toolbox
    {
        public readonly int COLOR_CLEAR = 255;
        public readonly int BRUSH_SIZE = 16;

        public toolbox(pixel[] p, int width, int height, int total)
        {
            pixels = p;
            imageWidth = width;
            totalPixels = total;
            imageHeight = height;
            brushRelativeMin = BRUSH_SIZE / 2 - 1;
            brushRelativeMax = BRUSH_SIZE / 2;
        }

        //gets some info for saving data, then taps run()
        public void clean(ProgressBar progressBar1)
        {
            if (pixels == null) {
                MessageBox.Show("No Pixels Loaded", "no pixels", 0);
                return;
            }
            run(progressBar1);
        }

        //the big boy, iterates through the pixels and drives algorithms
        private void run(ProgressBar progressBar1)
        {
            selection s = new selection(pixels, imageWidth, totalPixels);
            for (int i = 0; i < totalPixels; i++)
            {
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
                Console.WriteLine("x: " +  mapCleanup.walkPath[i].x.ToString() + " y: " + mapCleanup.walkPath[i].y.ToString()); 
        }

        // removes the path that was walked by using the the walk path genertated from the trajectory file in fileops
        public void removeDebris(fileOps mapCleanup)
        {
            for (int i = 0; i < mapCleanup.walkPath.Count(); i++)
                Brush(((imageHeight - mapCleanup.walkPath[i].y - 1) * imageWidth) + mapCleanup.walkPath[i].x);
        }

        // changes the color of the pixels and sets the touch value
        private void Brush(int trajectoryLocation)
        {
            for (int j = -(brushRelativeMin); j < brushRelativeMax; j++)
            {
                for (int k = -(brushRelativeMin); k < brushRelativeMax; k++)
                {
                    int pixelLocation = trajectoryLocation + ((imageWidth * j) + k);
                    if (!pixels[pixelLocation].selected) {
                        pixels[pixelLocation].value = Convert.ToByte(COLOR_CLEAR);
                        pixels[pixelLocation].selected = true;
                    }
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
        private int imageWidth, totalPixels, imageHeight, brushRelativeMin, brushRelativeMax;
        private List<int> buffer = new List<int>();
        private List<int> perimeter = new List<int>();
        private List<objectData> objdat = new List<objectData>();
    }
}