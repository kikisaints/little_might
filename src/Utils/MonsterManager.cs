using Little_Might.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Little_Might.Utils
{
    internal class MonsterManager
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
            allItemsList.Load(@"..\..\..\Content\data\MonsterData.xml");

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
                        monster.Sprite = content.Load<Texture2D>($"images/{childNode.InnerText}");
                    else if (itemAttribute == "InteractionSprite")
                        monster.InteractionSprite = content.Load<Texture2D>($"images/{childNode.InnerText}");
                    else if (itemAttribute == "GameColor")
                        monster.ObjectColor = GameColors.GetColorByStringName(childNode.InnerText);
                    else if (itemAttribute == "HP")
                        monsterStats.HP = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "SP")
                        monsterStats.Stamina = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "MP")
                        monsterStats.Mana = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "SPEED")
                        monsterStats.Speed = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "INT")
                        monsterStats.INT = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "STR")
                        monsterStats.STR = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "WIS")
                        monsterStats.WIS = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "DEX")
                        monsterStats.DEX = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "CHAR")
                        monsterStats.CHARISMA = int.Parse(childNode.InnerText);
                    else if (itemAttribute == "DEF")
                        monsterStats.DEFENSE = float.Parse(childNode.InnerText);
                    else if (itemAttribute == "CRIT")
                        monsterStats.CRIT = float.Parse(childNode.InnerText);
                    else if (itemAttribute == "Drop")
                    {
                        monster.ItemDrop = childNode.InnerText;
                    }
                }

                monster.Stats = monsterStats;
                AllMonsters.Add(monster);
            }
        }

        public static Inventory.ITEMTYPE GetMonsterDrop(Color monsterColor)
        {
            Inventory.ITEMTYPE itemType;
            Enum.TryParse(AllMonsters.Find(monster => monster.ObjectColor == monsterColor).ItemDrop, out itemType);

            foreach (Monster m in AllMonsters)
            {
                if (m.ObjectColor == monsterColor)
                    Enum.TryParse(m.ItemDrop.ToUpper(), out itemType);
            }

            return itemType;
        }

        public void AddMonsterToWorld(int numberOfMonsters, string monsterName, Utils.WorldMap.DungeonMap dunMap, int drawLayer = 0)
        {
            for (int i = 0; i < numberOfMonsters; i++)
            {
                Vector2 sPosition = _map.GetRandomGrassPoint();

                if (monsterName == "Goblin")
                    sPosition = _map.GetRandomDungeonMapPoint("SKAME RAGH");
                else if (monsterName == "Mole Rat")
                    sPosition = _map.GetRandomDungeonMapPoint("NORLOCKE");

                Monster ListMonster = AllMonsters.Find(monster => monster.Name.Equals(monsterName));
                Monster newMonster = new Monster()
                {
                    Sprite = ListMonster.Sprite,
                    InteractionSprite = ListMonster.InteractionSprite,
                    Stats = new Stats(ListMonster.Stats.HP,
                        ListMonster.Stats.Stamina,
                        ListMonster.Stats.Mana,
                        ListMonster.Stats.Speed,
                        ListMonster.Stats.INT,
                        ListMonster.Stats.STR,
                        ListMonster.Stats.WIS,
                        ListMonster.Stats.DEX,
                        ListMonster.Stats.CHARISMA,
                        ListMonster.Stats.DEFENSE,
                        ListMonster.Stats.CRIT
                        ),
                    Position = sPosition,
                    ObjectColor = ListMonster.ObjectColor,
                    MonsterType = ListMonster.MonsterType,
                    ItemDrop = ListMonster.ItemDrop,
                    Name = ListMonster.Name,
                    MonsterDungeonMap = dunMap,
                    DrawLayer = drawLayer
                };

                if (newMonster.MonsterType == Modules.Monster.MONSTERTYPE.CELESTIALHORROR)
                {
                    newMonster.Position = _map.GetRandomForestPoint();
                    newMonster.MoveTime = 0;
                }

                _graphicsManager.AddCharacterObject(newMonster);
                _worldMonsters.Add(newMonster);
            }
        }

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager, Utils.WorldMap wMap)
        {
            LoadAllMonsters(content);

            _worldMonsters = new List<Modules.Monster>();
            _graphicsManager = gManager;
            _content = content;
            _map = wMap;

            AddMonsterToWorld(_maxSlimeCount, "Slime", _map.GetDungeonByName(""));
            AddMonsterToWorld(_maxDeerCount, "Deer", _map.GetDungeonByName(""));
            AddMonsterToWorld(_maxRabbitCount, "Rabbit", _map.GetDungeonByName(""));

            WorldMap.DungeonMap dungeonMap = _map.GetDungeonByName("SKAME RAGH");
            if (dungeonMap.DungeonName != null)
                AddMonsterToWorld(10, "Goblin", _map.GetDungeonByName("SKAME RAGH"), 1);

            dungeonMap = _map.GetDungeonByName("NORLOCKE");
            if (dungeonMap.DungeonName != null)
                AddMonsterToWorld(5, "Mole Rat", _map.GetDungeonByName("NORLOCKE"), 1);

            AddMonsterToWorld(1, "Celestial Horror", _map.GetDungeonByName(""));
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
