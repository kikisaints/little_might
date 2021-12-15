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

        private double _timer = 0;
        private double _turnWaitTime = 1.5;
        private string _monstersTurnAction = "";

        public Stats Stats
        {
            get { return _stats; }
        }

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
            _stats = new AdvancedStats(25, 10, 0, 1, 1, 1, 1, 1, 1, 2f, 0.05f);

            _monsterType = type;
            Sprite = contentManager.Load<Texture2D>(spriteName);
            _interactionSprite = contentManager.Load<Texture2D>(intractSpriteName);
            Position = startingPosition;
            ObjectColor = color;
        }

        public void UpdateMonster(GameTime gTime, Game1 game, Utils.GraphicsManager gManager)
        {
            _timer += gTime.ElapsedGameTime.TotalSeconds;            

            if (_timer >= _turnWaitTime)
            {
                _timer = 0;
                if (_monstersTurnAction == "")
                {
                    _monstersTurnAction = "spit";
                    gManager.ShowSystemMessage(_monsterType.ToString().ToUpper() + " used " + _monstersTurnAction.ToUpper() + "!");
                }
                else
                {
                    game.EndMonsterTurn(_monstersTurnAction);
                    _monstersTurnAction = "";
                }
            }
        }
    }
}
