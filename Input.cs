using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections; //For hashtable
using System.Windows.Forms;

namespace Snake_Game
{
    class Input
    {
        //The Input class allows us know what key is being pressed in real time
        private static Hashtable keyTable = new Hashtable(); //This hashtable here will store the state of the keys

        public static bool KeyPress(Keys key)
        {
            if (keyTable[key] == null)
            {
                return false;
            }

            return (bool)keyTable[key];
        }

        public static void changeState(Keys key, bool state)
        {
            keyTable[key] = state;
        }
    }
}
