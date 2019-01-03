using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sharpclean
{
    class edge
    {
        edge() {
            Console.WriteLine(edge_warn + "edge initialized with no pixels\n");
        }

        public edge(int w, int t)
        {
            sel = null;
            per = null;
            edg = null;
            numEdges = 0;
            tolerance = 0;
            perimSize = 0;
            fieldSet = false;
            width = w;
            total = t;
        }

        // takes the full selected object and the root node of the binary tree buffer
        public void detect(List<int> mselection, node buff)
        {
            sel = buff;
            stack.Add(mselection[0]); // the first pixel is always an edge, add it to stack and perimeter/tree
            perimeter.Add(mselection[0]);
            tree.insert(ref per, mselection[0]);
            perimSize++;
            numEdges++;
            iterateEdges(); // begin iterating to find edges

            per = null;
            sel = null;
        }

        private void iterateEdges()
        {
            while (stack.Count != 0) // while the stack isn't empty, pop the top pixel off and use it to find the next edge pixel if one exists
            {
                int id = stack[stack.Count - 1]; // pop
                stack.RemoveAt(stack.Count - 1);
                getOctan(id); // get this pixel's octan
            }
        }

        private void getOctan(int id)
        {
            // create an octan and populate it with pixels, a pixel id of -1 means it doesn't exist in the selection (not necessarily in the pixel array)
            octan oct = new octan();

            if ((id - width) > 0) // check top left/center/right
            {
                oct.tl = tree.findNode(sel, id - width - 1);
                oct.t = tree.findNode(sel, id - width);
                oct.tr = tree.findNode(sel, id - width + 1);
            }

            if (id % width != 0) // check left
                oct.l = tree.findNode(sel, id - 1);

            if ((id + 1) % width != 0) // check right
                oct.r = tree.findNode(sel, id + 1);

            if ((id + width) < total) // check bottom left/center/right
            {
                oct.bl = tree.findNode(sel, id + width - 1);
                oct.b = tree.findNode(sel, id + width);
                oct.br = tree.findNode(sel, id + width + 1);
            }

            // now see if the octan contains an edge pixel, same order as above
            if ((id - width) > 0)
            { //read: "check if top left is an edge"
                check(oct.tl, oct.t, oct.l, n.tl);
                check(oct.t, oct.tl, oct.tr, n.t);
                check(oct.tr, oct.t, oct.r, n.tr);
            }

            if (id % width != 0)
                check(oct.l, oct.tl, oct.bl, n.l);

            if ((id + 1) % width != 0)
                check(oct.r, oct.tr, oct.br, n.r);

            if ((id + width) < total)
            {
                check(oct.bl, oct.l, oct.b, n.bl);
                check(oct.b, oct.bl, oct.br, n.b);
                check(oct.br, oct.b, oct.r, n.br);
            }
        }

        private void check(int p, int p1, int p2, int mneighbor)
        {
            if (p != -1 && !(p1 != -1 && p2 != -1)) // if p exists and p1 or p2 does not exist
            {
                if (tree.insert(ref per, p)) // insert it as a perimeter pixel, add to stack to find more, set field and run a tolerance check for edge compression
                {
                    stack.Add(p);
                    perimeter.Add(p);
                    perimSize++;

                    if (!fieldSet) {
                        setField(mneighbor);
                        numEdges++;
                    }
                    else
                    {
                        tolerance += field[mneighbor];
                        if (tolerance < -4 || tolerance > 4) //if a perimeter pixel is too far from the original line then we want to consider it the start of a new edge
                        {
                            tolerance = 0;
                            numEdges++;
                            setField(mneighbor);
                        }
                    }
                }
            }
        }

        private void setField(int mneighbor)
        {   // a field assigns values for the tolerance check
            if (mneighbor == n.t || mneighbor == n.b)
            {   //vertical
                field[n.tl] = -1;
                field[n.l] = -2;
                field[n.bl] = -1;
                field[n.t] = 0;
                field[n.b] = 0;
                field[n.tr] = 1;
                field[n.r] = 2;
                field[n.br] = 1;
                fieldSet = true;
            }

            else if (mneighbor == n.l || mneighbor == n.r)
            { //horizontal
                field[n.tl] = -1;
                field[n.t] = -2;
                field[n.tr] = -1;
                field[n.l] = 0;
                field[n.r] = 0;
                field[n.bl] = 1;
                field[n.b] = 2;
                field[n.br] = 1;
                fieldSet = true;
            }
            else if (mneighbor == n.tl || mneighbor == n.br)
            { //leftslant
                field[n.t] = -1;
                field[n.tr] = -2;
                field[n.r] = -1;
                field[n.tl] = 0;
                field[n.br] = 0;
                field[n.l] = 1;
                field[n.bl] = 2;
                field[n.b] = 1;
                fieldSet = true;
            }
            else
            { //rightslant
                field[n.l] = -1;
                field[n.tl] = -2;
                field[n.t] = -1;
                field[n.tr] = 0;
                field[n.bl] = 0;
                field[n.b] = 1;
                field[n.br] = 2;
                field[n.r] = 1;
                fieldSet = true;
            }
        }

        public List<int> getPerimiter()
        {
            return perimeter;
        }

        public int getSizeofPerimeter()
        {
            return perimSize;
        }

        public int getEdges()
        {
            return numEdges;
        }

        private node sel = new node(); // selection tree
        private node per = new node(); // perimeter tree
        private node edg = new node(); // edge tree
        private int[] field = new int[8]; // a field that will have set values for compressing perimeter pixels into edges
        private bool fieldSet;
        private List<int> perimeter = new List<int>();
        private List<int> stack = new List<int>();
        private int numEdges, perimSize, width, total, tolerance;
        private neighbor n = new neighbor();
        private readonly string edge_warn = "::EDGE::warning : ";
    }
}