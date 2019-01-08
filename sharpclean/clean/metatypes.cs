using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Sharp Clean: clean/metatypes.cs
 * Author: Austin Herman
 */

namespace sharpclean
{
    //the .pgm file's basic data
    struct data
    {
        public string filetype;
        public int width, height, maxgreyval;
        public int totalpixels;
    };

    //used for the menu's when the user selects to print certain data
    enum info
    {
        ALL, FILETYPE, DIMENSIONS, TOTALPIXELS, MAXGREYVAL
    };

    //a basic pixel class
    struct pixel
    {
        public bool selected;   //used for selection
        public bool found;      //used in filler
        public byte value;      //grey value
        public int id;          //ID [0->totalpixels]
    };

    //edge and filler use this for navigation around the pixel map
    enum direction
    {
        none, up, down, left, right
    };

    //same deal, a little more abstract
    class pathDirection
    {
        public direction dir = direction.none;
        public int id = -1;
        public pathDirection(direction d, int i) { dir = d; id = i; }
    };

    //each pixel has eight neighbors
    class octan
    {
        public int tl = -1, t = -1, tr = -1,
                   l = -1, r = -1,
                   bl = -1, b = -1, br = -1;

        public octan()
        {
            tl = -1; t = -1; tr = -1;
            l = -1; r = -1;
            bl = -1; b = -1; br = -1;
        }
    };

    //same deal, different use
    class neighbor
    {
        public readonly int tl = 0;
        public readonly int t = 1;
        public readonly int tr = 2;
        public readonly int l = 3;
        public readonly int r = 4;
        public readonly int bl = 5;
        public readonly int b = 6;
        public readonly int br = 7;
    };

    //confidence class
    public class conf
    {
        public double obj = 0.0, dust = 0.0,
                        o_size = 0.0, d_size = 0.0,
                        o_edge = 0.0, d_edge = 0.0,
                        o_val = 0.0, d_val = 0.0;
        public bool isObj;
    };

    public class objectData
    {
        public objectData(double av, int s, double e)
        {
            avgval = av;
            size = s;
            edgeratio = e;
        }

        public double avgval;
        public int size;
        public double edgeratio;
        public conf objconf;
    }

    //for building the tree
    class tup
    {
        public int s, e;
        public tup(int st, int en) { s = st; e = en; }
        public void change(int st, int en) { s = st; e = en; }
    };
}