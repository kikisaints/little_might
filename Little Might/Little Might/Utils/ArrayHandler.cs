using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Utils
{
    class ArrayHandler
    {
        public static Vector2 Get2DPoint(int index, int size)
        {
            return new Vector2(index % size, index / size);
        }

        public static int Get1DIndex(int x, int y, int size)
        {
            return (int)((x) * size + (y));
        }
    }
}
