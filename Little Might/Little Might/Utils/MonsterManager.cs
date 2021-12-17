using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Utils
{
    class MonsterManager
    {
        private Utils.GraphicsManager _graphicsManager;
        private ContentManager _content;
        private List<Modules.Monster> _allMonsters;
        private Utils.WorldMap _map;

        //Need to adjust these numbers based on map size...
        private int _maxSlimeCount = 100;
        private int _maxDeerCount = 50;
        private int _maxRabbitCount = 100;

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager, Utils.WorldMap wMap)
        {
            _allMonsters = new List<Modules.Monster>();
            _graphicsManager = gManager;
            _content = content;
            _map = wMap;

            SetupMonsters(_maxSlimeCount, "monster_slime", "slime_interaction_img", Modules.Monster.MONSTERTYPE.SLIME, GameColors.MonsterSlimeColor);
            SetupMonsters(_maxDeerCount, "deer", "monster_deer", Modules.Monster.MONSTERTYPE.DEER, GameColors.MonsterDeerColor);
            SetupMonsters(_maxRabbitCount, "rabbit", "monster_rabbit", Modules.Monster.MONSTERTYPE.RABBIT, GameColors.MonsterRabbitColor);
        }

        private void SetupMonsters(int count, string worldName, string interactionName, Modules.Monster.MONSTERTYPE type, Color monsterColor)
        {
            for (int i = 0; i < count; i++)
            {
                Modules.Monster testMonster = new Modules.Monster(worldName, interactionName, _map.GetRandomGrassPoint(), _content, monsterColor, type);
                _graphicsManager.AddCharacterObject(testMonster);

                _allMonsters.Add(testMonster);
            }
        }

        public void UpdateMonsters(GameTime time, bool overworldActive = true)
        {
            if (overworldActive)
            {
                foreach (Modules.Monster monster in _allMonsters)
                {
                    monster.UpdateMovement(time, _map);
                }
            }
        }
    }
}
