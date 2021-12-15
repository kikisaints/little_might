using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;

namespace Little_Might.Utils
{
    class GameColors
    {
        static private Color _mainBackgroundColor = new Color(18, 19, 22);
        static private Color _mountainMapColor = Color.Gray;
        static private Color _rockMapColor = Color.DarkGray;
        static private Color _bushMapColor = new Color(45, 138, 6, 255);
        static private Color _evergreenForestMapColor = new Color(16, 77, 7, 255);
        static private Color _treeForestMapColor = new Color(54, 87, 22, 255);
        static private Color _treeFruitMapColor = new Color(72, 145, 41, 255);
        static private Color _grassMapColor = new Color(145, 212, 63, 255);
        static private Color _herbMapColor = new Color(145, 213, 63, 255);
        static private Color _waterMapColor = new Color(66, 135, 245, 255);
        static private Color _campfireMapColor = Color.IndianRed;
        static private Color _monsterSlimeColor = new Color(3, 132, 256, 255);

        static public Color MonsterSlimeColor
        {
            get { return _monsterSlimeColor; }
        }

        static public Color HerbMapColor
        {
            get { return _herbMapColor; }
        }

        static public Color CampfireMapColor
        {
            get { return _campfireMapColor; }
        }

        static public Color MainBackgroundColor
        {
            get { return _mainBackgroundColor; }
        }

        static public Color MountainMapColor
        {
            get { return _mountainMapColor; }
        }

        static public Color EvergreenForestMapColor
        {
            get { return _evergreenForestMapColor; }
        }

        static public Color TreeForestMapColor
        {
            get { return _treeForestMapColor; }
        }

        static public Color TreeFruitMapColor
        {
            get { return _treeFruitMapColor; }
        }

        static public Color GrassMapColor
        {
            get { return _grassMapColor; }
        }

        static public Color WaterMapColor
        {
            get { return _waterMapColor; }
        }

        static public Color RockMapColor
        {
            get { return _rockMapColor; }
        }

        static public Color BushMapColor
        {
            get { return _bushMapColor; }
        }
    }
}
