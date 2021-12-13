using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Utils
{
    class MathHandler
    {
        public static Vector2 Get2DPoint(int index, int size)
        {
            return new Vector2(index % size, index / size);
        }

        public static int Get1DIndex(int x, int y, int size)
        {
            return (int)((x) * size + (y));
        }

        public static bool IsPointInCircle(int x, int y, int centerX, int centerY, int radius)
        {
            int pointCheck = ((x - centerX) * (x - centerX)) + ((y - centerY) * (y - centerY));

            if (pointCheck <= (radius * radius))
                return true;

            return false;
        }
    }
}
