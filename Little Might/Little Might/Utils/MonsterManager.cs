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

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager)
        {
            _graphicsManager = gManager;
            _content = content;            
        }

        public void UpdateMonsters(Vector2 playerPos, GameTime time)
        {
            if (!spawned)
            {
                spawned = true;
                _graphicsManager.AddCharacterObject(new Modules.Monster("monster_slime", playerPos + new Vector2(9,0), _content, Utils.GameColors.MonsterSlimeColor));
            }
        }
    }
}
