using Little_Might.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Little_Might.Utils
{
    class MonsterManager
    {
        private Utils.GraphicsManager _graphicsManager;
        private ContentManager _content;
        private List<Modules.Monster> _worldMonsters;
        public static List<Modules.Monster> AllMonsters;
        private Utils.WorldMap _map;

        //Need to adjust these numbers based on map size...
        private int _maxSlimeCount = 100;
        private int _maxDeerCount = 50;
        private int _maxRabbitCount = 100;

        private static void LoadAllMonsters(ContentManager content)
        {
            AllMonsters = new List<Monster>();

            XmlDocument allItemsList = new XmlDocument();
            allItemsList.Load(@"..\netcoreapp3.1\Content\data\MonsterData.xml");

            XmlNodeList nodes = allItemsList.SelectNodes("//Monsters/Monster");

            foreach (XmlNode node in nodes)
            {
                Monster monster = new Monster();
                Stats monsterStats = new Stats();

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    string itemAttribute = childNode.Name;
                    Monster.MONSTERTYPE checkType;

                    if (itemAttribute == "Name")
                        monster.Name = childNode.InnerText;
                    else if (itemAttribute == "MonsterType")
                    {
                        if (Enum.TryParse(childNode.InnerText.ToUpper(), out checkType))
                            monster.MonsterType = checkType;
                    }
                    else if (itemAttribute == "WorldSprite")
                        monster.Sprite = content.Load<Texture2D>(childNode.InnerText);
                    else if (itemAttribute == "InteractionSprite")
                        monster.InteractionSprite = content.Load<Texture2D>(childNode.InnerText);
                    else if (itemAttribute == "GameColor")
                        monster.ObjectColor = GameColors.GetColorByStringName(childNode.InnerText);
                    else if (itemAttribute == "HP")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "SP")
                        monsterStats.Stamina = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "MP")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "SPEED")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "INT")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "STR")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "WIS")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "DEX")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "CHAR")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "DEF")
                        monsterStats.DEFENSE = float.Parse(childNode.InnerText);
                    else if (itemAttribute == "CRIT")
                        monsterStats.CRIT = float.Parse(childNode.InnerText);
                    else if (itemAttribute == "Drop")
                        monster.ItemDrop = childNode.InnerText;
                }

                monster.Stats = monsterStats;
                AllMonsters.Add(monster);
            }
        }

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager, Utils.WorldMap wMap)
        {
            LoadAllMonsters(content);

            _worldMonsters = new List<Modules.Monster>();
            _graphicsManager = gManager;
            _content = content;
            _map = wMap;

            SetupMonsters(_maxSlimeCount, "monster_slime", "slime_interaction_img", Modules.Monster.MONSTERTYPE.SLIME, GameColors.MonsterSlimeColor, new Stats(25, 10, 0, 1, 1, 1, 1, 1, 1, 2f, 0f));
            SetupMonsters(_maxDeerCount, "deer", "monster_deer", Modules.Monster.MONSTERTYPE.DEER, GameColors.MonsterDeerColor, new Stats(50, 20, 0, 1, 1, 1, 1, 1, 1, 5f, 0f));
            SetupMonsters(_maxRabbitCount, "rabbit", "monster_rabbit", Modules.Monster.MONSTERTYPE.RABBIT, GameColors.MonsterRabbitColor, new Stats(15, 10, 0, 1, 1, 1, 1, 1, 1, 0f, 0f));
            SetupMonsters(1, "celestialhorror", "monster_celestialhorror", Modules.Monster.MONSTERTYPE.CELESTIALHORROR, GameColors.MonsterCelestialHorrorColor, new Stats(1000, 100, 450, 10, 100, 100, 100, 100, 100, 50f, 50f));
        }

        private void SetupMonsters(int count, string worldName, string interactionName, Modules.Monster.MONSTERTYPE type, Color monsterColor, Stats stats)
        {
            for (int i = 0; i < count; i++)
            {
                Modules.Monster testMonster = new Modules.Monster(worldName, interactionName, _map.GetRandomGrassPoint(), _content, monsterColor, type, stats);
                if (type == Modules.Monster.MONSTERTYPE.CELESTIALHORROR)
                {
                    testMonster.Position = _map.GetRandomForestPoint();
                    testMonster.MoveTime = 0;
                }

                _graphicsManager.AddCharacterObject(testMonster);

                _worldMonsters.Add(testMonster);
            }
        }

        public void UpdateMonsters(GameTime time, bool overworldActive = true)
        {
            if (overworldActive)
            {
                foreach (Modules.Monster monster in _worldMonsters)
                {
                    monster.UpdateMovement(time, _map);
                }
            }
        }
    }
}
