using Microsoft.Xna.Framework;

namespace Little_Might.Utils
{
    internal class GameColors
    {
        private static Color _mainBackgroundColor = new Color(18, 19, 22);
        private static Color _mountainMapColor = Color.Gray;
        private static Color _rockMapColor = Color.DarkGray;
        private static Color _bushMapColor = new Color(45, 138, 6, 255);
        private static Color _evergreenForestMapColor = new Color(16, 77, 7, 255);
        private static Color _treeForestMapColor = new Color(54, 87, 22, 255);
        private static Color _treeFruitMapColor = new Color(72, 145, 41, 255);
        private static Color _grassMapColor = new Color(145, 212, 63, 255);
        private static Color _herbMapColor = new Color(145, 213, 63, 255);
        private static Color _waterMapColor = new Color(66, 135, 245, 255);
        private static Color _campfireMapColor = Color.IndianRed;

        private static Color _monsterSlimeColor = new Color(3, 132, 256, 255);
        private static Color _monsterDeerColor = new Color(235, 172, 101, 255);
        private static Color _monsterRabbitColor = new Color(255, 204, 232, 255);
        private static Color _monsterCelestialHorrorColor = Color.Red;
        private static Color _monsterGoblinColor = new Color(36, 255, 131);

        private static Color _furnaceMapColor = Color.Orange;
        private static Color _chestMapColor = new Color(168, 117, 50);
        private static Color _prarieDungeonMapColor = new Color(62, 85, 115);
        private static Color _stoneMapColor = new Color(116, 138, 237);

        public static Color GetColorByStringName(string name)
        {
            switch (name)
            {
                case "MonsterSlimeColor":
                    return _monsterSlimeColor;

                case "MonsterRabbitColor":
                    return _monsterRabbitColor;

                case "MonsterDeerColor":
                    return _monsterDeerColor;

                case "MonsterGoblinColor":
                    return _monsterGoblinColor;

                case "MonsterCelestialHorrorColor":
                    return _monsterCelestialHorrorColor;

                default:
                    return _mainBackgroundColor;
            }
        }

        public static Color MonsterGoblinColor
        {
            get { return _monsterGoblinColor; }
        }

        public static Color StoneMapColor
        {
            get { return _stoneMapColor; }
        }

        public static Color PrarieDungeonMapColor
        {
            get { return _prarieDungeonMapColor; }
        }

        public static Color ChestMapColor
        {
            get { return _chestMapColor; }
        }

        public static Color FurnaceMapColor
        {
            get { return _furnaceMapColor; }
        }

        public static Color MonsterDeerColor
        {
            get { return _monsterDeerColor; }
        }

        public static Color MonsterRabbitColor
        {
            get { return _monsterRabbitColor; }
        }

        public static Color MonsterCelestialHorrorColor
        {
            get { return _monsterCelestialHorrorColor; }
        }

        public static Color MonsterSlimeColor
        {
            get { return _monsterSlimeColor; }
        }

        public static Color HerbMapColor
        {
            get { return _herbMapColor; }
        }

        public static Color CampfireMapColor
        {
            get { return _campfireMapColor; }
        }

        public static Color MainBackgroundColor
        {
            get { return _mainBackgroundColor; }
        }

        public static Color MountainMapColor
        {
            get { return _mountainMapColor; }
        }

        public static Color EvergreenForestMapColor
        {
            get { return _evergreenForestMapColor; }
        }

        public static Color TreeForestMapColor
        {
            get { return _treeForestMapColor; }
        }

        public static Color TreeFruitMapColor
        {
            get { return _treeFruitMapColor; }
        }

        public static Color GrassMapColor
        {
            get { return _grassMapColor; }
        }

        public static Color WaterMapColor
        {
            get { return _waterMapColor; }
        }

        public static Color RockMapColor
        {
            get { return _rockMapColor; }
        }

        public static Color BushMapColor
        {
            get { return _bushMapColor; }
        }
    }
}
