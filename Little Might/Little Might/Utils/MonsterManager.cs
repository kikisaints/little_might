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

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager, Vector2 playerPos)
        {
            _allMonsters = new List<Modules.Monster>();
            _graphicsManager = gManager;
            _content = content;

            Modules.Monster testMonster = new Modules.Monster("monster_slime", "slime_interaction_img", playerPos + new Vector2(18, 9), _content, Utils.GameColors.MonsterSlimeColor, Modules.Monster.MONSTERTYPE.SLIME);
            _graphicsManager.AddCharacterObject(testMonster);

            _allMonsters.Add(testMonster);
        }

        public void UpdateMonsters(GameTime time, bool overworldActive = true)
        {

        }
    }
}
