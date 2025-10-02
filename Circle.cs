using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Game
{
    class Circle 
    {
        //This class gives the X and Y location of the snake object
        public int X { get; set; }
        public int Y { get; set; }

        public Circle() { //Gives the default values of X and Y

            X = 0;
            Y = 0;
        }
    }
}
