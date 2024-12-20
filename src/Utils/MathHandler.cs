﻿using Microsoft.Xna.Framework;
using System;

namespace Little_Might.Utils
{
    internal class MathHandler
    {
        private static Random _randomNumber = new Random();

        public static int GetRandomNumber(int minValue, int maxValue)
        {
            return _randomNumber.Next(minValue, maxValue + 1);
        }

        public static Vector2 Get2DPoint(int index, int size)
        {
            return new Vector2(index % size, index / size);
        }

        public static int Get1DIndex(int x, int y, int size)
        {
            return (int)((x * size) + y);
        }

        public static bool IsPointInCircle(int x, int y, int centerX, int centerY, int radius)
        {
            int pointCheck = ((x - centerX) * (x - centerX)) + ((y - centerY) * (y - centerY));

            if (pointCheck <= (radius * radius))
                return true;

            return false;
        }

        public static Color BlendColor(Color topColor, Color backColor, double amount)
        {
            byte r = (byte)((topColor.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((topColor.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((topColor.B * amount) + backColor.B * (1 - amount));

            return new Color(r, g, b, (byte)255);
        }

        public static bool WorldObjectIntersects(Modules.WorldObject objA, Modules.WorldObject objB)
        {
            return WorldObjectIntersects(objA.Position, objB.Position);
        }

        public static bool WorldObjectIntersects(Vector2 objA, Vector2 objB)
        {
            Rectangle rectA = new Rectangle((int)objA.X, (int)objA.Y, Utils.WorldMap.UNITSIZE, Utils.WorldMap.UNITSIZE);
            Rectangle rectB = new Rectangle((int)objB.X, (int)objB.Y, Utils.WorldMap.UNITSIZE, Utils.WorldMap.UNITSIZE);

            return rectA.Intersects(rectB);
        }
    }
}
