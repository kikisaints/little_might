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
        public enum MONSTERTYPE
        {
            SLIME = 0
        }

        private Stats _stats;
        private Texture2D _interactionSprite;
        private MONSTERTYPE _monsterType;

        public Texture2D InteractionSprite
        {
            get { return _interactionSprite; }
        }

        public MONSTERTYPE MonsterType
        {
            get { return _monsterType; }
        }

        public Monster(string spriteName, string intractSpriteName, Vector2 startingPosition, ContentManager contentManager, Color color, MONSTERTYPE type)
        {
            _stats = new AdvancedStats(5, 10, 0, 1, 1, 1, 1, 1, 1, 0.05f);

            _monsterType = type;
            Sprite = contentManager.Load<Texture2D>(spriteName);
            _interactionSprite = contentManager.Load<Texture2D>(intractSpriteName);
            Position = startingPosition;
            ObjectColor = color;
        }
    }
}
