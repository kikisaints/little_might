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

        private int _maxSlimeCount = 100;        

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager, Utils.WorldMap wMap)
        {
            _allMonsters = new List<Modules.Monster>();
            _graphicsManager = gManager;
            _content = content;
            _map = wMap;

            SetUpSlimes();
        }

        private void SetUpSlimes()
        {
            for (int i = 0; i < _maxSlimeCount; i++)
            {
                Modules.Monster testMonster = new Modules.Monster("monster_slime", "slime_interaction_img", _map.GetRandomGrassPoint(), _content, Utils.GameColors.MonsterSlimeColor, Modules.Monster.MONSTERTYPE.SLIME);
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
