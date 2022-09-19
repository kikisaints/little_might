using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Little_Might.Utils
{
    class ADungeonCreator
    {
        static private Random rand = new Random();

        private enum DIRECTION
        {
            NORTH = 0,
            SOUTH,
            EAST,
            WEST
        }

        static public int[] GenerateDungeon(int size, int availCells)
        {
            int[,] genDungeon = new int[size, size];

            //Get the starting position
            int startPointX = rand.Next(0, size);
            int startPointY = rand.Next(0, size);
            genDungeon[startPointX, startPointY] = 8;
            int filledCells = 0;
            int attempts = 100;
            bool chestPlaced = false;

            Point newPoint = new Point(startPointX, startPointY);
            while (filledCells <= availCells)
            {
                switch (GetNextCellPosition())
                {
                    case DIRECTION.NORTH:
                        newPoint.Y--;
                        break;
                    case DIRECTION.SOUTH:
                        newPoint.Y++;
                        break;
                    case DIRECTION.WEST:
                        newPoint.X--;
                        break;
                    case DIRECTION.EAST:
                        newPoint.X++;
                        break;
                    default:
                        newPoint.Y--;
                        break;
                }

                if (newPoint.X < 0 || newPoint.X >= size ||
                    newPoint.Y < 0 || newPoint.Y >= size)
                {
                    newPoint = new Point(startPointX, startPointY);
                    continue;
                }
                else
                {
                    if (genDungeon[newPoint.X, newPoint.Y] != 1 &&
                        genDungeon[newPoint.X, newPoint.Y] != 8)
                    {                        
                        genDungeon[newPoint.X, newPoint.Y] = 1;
                        filledCells++;
                    }
                    else
                    {
                        attempts--;

                        if (attempts <= 0)
                            break;
                    }
                }
            }

            int[] finalDun = new int[size * size];
            int i = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    finalDun[i] = genDungeon[y, x];

                    int chest = rand.Next(0, 2);

                    if (!chestPlaced && chest <= 1 && finalDun[i] == 1)
                    {
                        chestPlaced = true;
                        finalDun[i] = 2;
                    }

                    i++;
                }
            }

            return finalDun;
        }

        static private DIRECTION GetNextCellPosition()
        {
            return (DIRECTION)rand.Next(1, 4);
        }
    }
    class DungeonPresets
    {
        static private int[] SKAMERAGH_DUNGEON = ADungeonCreator.GenerateDungeon(28, 300);

        static private int[] NORLOCKE_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] UNDEACA_DUNGEON = ADungeonCreator.GenerateDungeon(50, 1000);

        static private int[] HAWKINEL_DUNGEON = ADungeonCreator.GenerateDungeon(10, 50);

        static private int[] EVANSANO_DUNGEON = ADungeonCreator.GenerateDungeon(25, 250);

        static private int[] THANARG_DUNGEON = ADungeonCreator.GenerateDungeon(35, 650);

        static private int[] MELKOG_DUNGEON = ADungeonCreator.GenerateDungeon(40, 700);

        static private int[] SARKATH_DUNGEON = ADungeonCreator.GenerateDungeon(15, 85);

        static private int[] VALISHA_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] DORYU_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] KAIDA_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] INKANUS_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] ZEPHYR_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] KAVARTHON_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] MOROPHELES_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] NERIT_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] SERARONG_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] REINIAR_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        static private int[] HENDERHELL_DUNGEON = ADungeonCreator.GenerateDungeon(20, 200);

        public static int[] GetDungeonMap(string name)
        {
            switch (name.ToUpper())
            {
                case "SKAME RAGH":
                    return SKAMERAGH_DUNGEON;
                case "NORLOCKE":
                    return NORLOCKE_DUNGEON;
                case "UNDEACA":
                    return UNDEACA_DUNGEON;
                case "HAWKINEL":
                    return HAWKINEL_DUNGEON;
                case "EVANSANO":
                    return EVANSANO_DUNGEON;
                case "THANARG":
                    return THANARG_DUNGEON;
                case "MELKOG":
                    return MELKOG_DUNGEON;
                case "SARKATH":
                    return SARKATH_DUNGEON;
                case "VALISHA":
                    return VALISHA_DUNGEON;
                case "DORYU":
                    return DORYU_DUNGEON;
                case "KAIDA":
                    return KAIDA_DUNGEON;
                case "INKANUS":
                    return INKANUS_DUNGEON;
                case "ZEPHYR":
                    return ZEPHYR_DUNGEON;
                case "KAVARTHON":
                    return KAVARTHON_DUNGEON;
                case "MOROPHELES":
                    return MOROPHELES_DUNGEON;
                case "NERIT":
                    return NERIT_DUNGEON;
                case "SERARONG":
                    return SERARONG_DUNGEON;
                case "REINIAR":
                    return REINIAR_DUNGEON;
                case "NEHENDERHELLRIT":
                    return HENDERHELL_DUNGEON;
                default:
                    return null;
            }
        }
    }
}
