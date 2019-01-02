using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sharpclean
{
    class selection
    {
        public const byte VALUE_THRESHOLD = 255;
        selection()
        {
            Console.Write(selection_err + "initialized without pixels\n");
        }

        public selection(pixel[] p, int width, int total)
        {
            buffer = new List<int>();
            perimeter = new List<int>();
            pixels = p;
            this.width = width;
            this.total = total;
            bufferSize = 0;
        }

        public bool get(int i)
        {
	        if (checkPixel(ref pixels[i], VALUE_THRESHOLD))
	        {
		        iterate(VALUE_THRESHOLD);

		        if (bufferSize > 0 && bufferSize< 2700)
		        {
			        fillPixels();
                    findEdges();
			        return true;
		        }
            }
	        return false;
        }



        private void iterate(int value)
        {
            for (int i = 0; i < bufferSize - 1; i++)
                nextPixel(buffer[i], value);
        }

        private void nextPixel(long i, int value)
        {
            if ((i - width - 1) > 0)
            {
                checkPixel(ref pixels[i - width - 1], value);   //top left
                checkPixel(ref pixels[i - width], value);       //top center
                checkPixel(ref pixels[i - width + 1], value);   //top right
            }

            if (i % width != 0)
                checkPixel(ref pixels[i - 1], value);   //center left
            if (i % (width + 1) != 0)
                checkPixel(ref pixels[i + 1], value);   //center right

            if ((i + width + 1) < total)
            {
                checkPixel(ref pixels[i + width - 1], value);   //bottom left
                checkPixel(ref pixels[i + width], value);       //bottom center
                checkPixel(ref pixels[i + width + 1], value);   //bottom right
            }
        }

        private bool checkPixel(ref pixel p, int value)
        {
            if (p.value < value)
            {
                if (!p.selected)
                {
                    buffer.Add(p.id);
                    p.selected = true;
                    bufferSize++;
                    tree.insert(buff, p.id);
                    return true;
                }
            }
            return false;
        }

        private void fillPixels()
        {
            filler fill = new filler(pixels, width, total);
            for (int i = 0; i < bufferSize; i++)
                fill.getBounds(buffer[i]);
            int count = 0;
            for (int i = 0; i < bufferSize; i++)
            {
                if (buffer[i] + width < total && !pixels[buffer[i] + width].selected && pixels[buffer[i] + width].value >= VALUE_THRESHOLD)
                {
                    fill.start(buffer[i] + width);
                    List<pathDirection> whitePixels = fill.getPath();
                    if (whitePixels.Count != 0)
                    {
                        //j once was i, not sure how this wasn't an issue in c++
                        //but reassigning i here may have slowed things down a lot

                        for (int j = 0; j < whitePixels.Count; j++)
                        {
                            buffer.Add(whitePixels[j].id);
                            tree.insert(buff, whitePixels[j].id);
                            count++;
                        }
                    }
                }
            }
            fill.clearFoundBuffer();
            bufferSize += count;
        }

        private void findEdges()
        {
            edge e = new edge(width, total);
            e.detect(buffer, buff);
            perimeter = e.getPerimiter();
            numEdges = e.getEdges();
        }

        public ref List<int> getBuffer()
        {
	        return ref buffer;
        }

        public ref List<int>  getPerimeter()
        {
	        return ref perimeter;
        }

        public int getEdges()
        {
            return numEdges;
        }

        public void clearBuffer()
        {
            buffer.Clear();
            bufferSize = 0;
            buff = null;
        }

        private pixel[] pixels;
        private int width, total;
        private List<int> buffer, perimeter;
        private int bufferSize, numEdges;
        private node buff;
        private readonly string selection_err = "::SELECTION::error : ";
    }
}