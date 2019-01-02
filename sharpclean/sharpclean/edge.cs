using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sharpclean
{
    class edge
    {
        edge()
        {
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

        public void detect(List<int> mselection, node buff)
        {
            /*
            node * temp = NULL;
            std::vector<int> balance;
            for (int i = 0; i < size; i++)
                Tree::insert(temp, selection[i]);

            Tree::getInOrder(temp, balance);

            Tree::deleteTree(temp);
                      
            Tree::buildTree(balance, sel);
            */

            sel = buff;
            stack.Add(mselection[0]);
            perimeter.Add(mselection[0]);
            tree.insert(per, mselection[0]);
            perimSize++;
            numEdges++;
            iterateEdges();

            //Tree::deleteTree(sel);

            //Tree::deleteTree(per);

            per = null;
            sel = null;

            //std::cout << numEdges << "\n";
        }

        private void iterateEdges()
        {
            while (stack.Count != 0)
            {
                int id = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                getOctan(id);
            }
        }

        private void getOctan(int id)
        {
            octan oct = new octan();
            if ((id - width) > 0)
            {
                oct.tl = tree.findNode(sel, id - width - 1);
                oct.t = tree.findNode(sel, id - width);
                oct.tr = tree.findNode(sel, id - width + 1);
            }

            if (id % width != 0)
                oct.l = tree.findNode(sel, id - 1);

            if ((id + 1) % width != 0)
                oct.r = tree.findNode(sel, id + 1);

            if ((id + width) < total)
            {
                oct.bl = tree.findNode(sel, id + width - 1);
                oct.b = tree.findNode(sel, id + width);
                oct.br = tree.findNode(sel, id + width + 1);
            }

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
            if (p == -1 && !(p1 == -1 && p2 == -1))
            {
                if (tree.insert(per, p))
                {
                    stack.Add(p);
                    perimeter.Add(p);
                    perimSize++;

                    if (!fieldSet)
                    {
                        setField(mneighbor);
                        numEdges++;
                    }
                    else
                    {
                        tolerance += field[mneighbor];
                        if (tolerance < -4 || tolerance > 4)
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
        {
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

        private node sel = new node();
        private node per = new node();
        private node edg = new node();
        private int[] field = new int[8];
        private bool fieldSet;
        private List<int> perimeter = new List<int>();
        private List<int> stack = new List<int>();
        private int numEdges, perimSize, width, total, tolerance;
        private neighbor n = new neighbor();
        private readonly string edge_warn = "::EDGE::warning : ";
    }
}