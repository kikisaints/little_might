using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Modules
{
    class Monster : WorldObject
    {
        private Stats _stats;

        public Monster(string spriteFileName, Vector2 startingPosition, ContentManager contentManager, Color color)
        {
            _stats = new AdvancedStats(5, 10, 0, 1, 1, 1, 1, 1, 1, 0.05f);

            Sprite = contentManager.Load<Texture2D>(spriteFileName);
            Position = startingPosition;
            ObjectColor = color;
        }
    }
}
