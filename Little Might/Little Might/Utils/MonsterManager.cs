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

        //TEST
        bool spawned = false;

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager, Vector2 playerPos)
        {
            _graphicsManager = gManager;
            _content = content;

            _graphicsManager.AddCharacterObject(new Modules.Monster("monster_slime", "slime_interaction_img", playerPos + new Vector2(18, 9), _content, Utils.GameColors.MonsterSlimeColor, Modules.Monster.MONSTERTYPE.SLIME));
        }

        public void UpdateMonsters(GameTime time)
        {
        }
    }
}
