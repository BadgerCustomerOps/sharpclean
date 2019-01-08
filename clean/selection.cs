using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Sharp Clean: clean/selection.cs
 * Author: Austin Herman
 */

namespace sharpclean
{
    class selection
    {
        public const byte VALUE_THRESHOLD = 255;
        public readonly int MAX_OBJECT_SIZE_ESTIMATE = 2700;    //if an object is bigger than this ignore it -- optimization thing

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
	        if (checkPixel(ref pixels[i]))
	        {
		        iterate();

		        if (bufferSize > 0 && bufferSize < MAX_OBJECT_SIZE_ESTIMATE)
		        {
			        fillPixels();
                    findEdges();
			        return true;
		        }
            }
	        return false;
        }

        private void iterate()
        {
            for (int i = 0; i < bufferSize; i++)
                nextPixel(buffer[i]);
        }

        private void nextPixel(long i)
        {
            if ((i - width - 1) > 0)
            {
                checkPixel(ref pixels[i - width - 1]);   //top left
                checkPixel(ref pixels[i - width]);       //top center
                checkPixel(ref pixels[i - width + 1]);   //top right
            }

            if (i % width != 0)
                checkPixel(ref pixels[i - 1]);   //center left
            if (i % (width + 1) != 0)
                checkPixel(ref pixels[i + 1]);   //center right

            if ((i + width + 1) < total)
            {
                checkPixel(ref pixels[i + width - 1]);   //bottom left
                checkPixel(ref pixels[i + width]);       //bottom center
                checkPixel(ref pixels[i + width + 1]);   //bottom right
            }
        }

        private bool checkPixel(ref pixel p)
        {
            if (p.value < VALUE_THRESHOLD)
            {
                if (!p.selected)
                {
                    buffer.Add(p.id);
                    p.selected = true;
                    bufferSize++;
                    tree.insert(ref buff, p.id);
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
                        for (int j = 0; j < whitePixels.Count; j++)
                        {
                            buffer.Add(whitePixels[j].id);
                            tree.insert(ref buff, whitePixels[j].id);
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

        public ref List<int> Buffer => ref buffer;

        public ref List<int> Perimeter => ref perimeter;

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