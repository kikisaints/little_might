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

        public static Inventory.ITEMTYPE GetDrop(MONSTERTYPE type)
        {
            switch(type.ToString().ToLower())
            {
                case "slime":
                    return Inventory.ITEMTYPE.GOOP;
            }

            return Inventory.ITEMTYPE.NONE;
        }

        private Stats _stats;
        private Texture2D _interactionSprite;
        private MONSTERTYPE _monsterType;

        private double _timer = 0;
        private double _turnWaitTime = 1.5;
        private string _monstersTurnAction = "";
        private Random _movementWaitTime = new Random();

        public double MoveTime = 0;

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

            MoveTime = _movementWaitTime.Next(1, 5) * 0.5;
        }

        public void UpdateMovement(GameTime gTime, Utils.WorldMap map)
        {
            _timer += gTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= MoveTime)
            {
                _timer = 0;
                int dir = Utils.MathHandler.GetRandomNumber(0, 4);
                Vector2 movePosition = Vector2.Zero;

                if (dir == 0)
                {
                    movePosition = new Vector2(Position.X + Utils.WorldMap.UNITSIZE, Position.Y);
                }
                else if (dir == 1)
                {
                    movePosition = new Vector2(Position.X, Position.Y + Utils.WorldMap.UNITSIZE);
                }
                else if (dir == 2)
                {
                    movePosition = new Vector2(Position.X, Position.Y - Utils.WorldMap.UNITSIZE);
                }
                else
                {
                    movePosition = new Vector2(Position.X - Utils.WorldMap.UNITSIZE, Position.Y);
                }

                if (_monsterType == MONSTERTYPE.SLIME && map.GetTileType(movePosition) == Utils.WorldMap.MAPTILETYPE.GRASS)
                    Position = movePosition;
            }
        }

        public void UpdateMonster(GameTime gTime, Game1 game, Utils.GraphicsManager gManager)
        {
            _timer += gTime.ElapsedGameTime.TotalSeconds;            

            if (_timer >= _turnWaitTime)
            {
                _timer = 0;
                if (_monstersTurnAction == "")
                {
                    _monstersTurnAction = Utils.CombatHandler.GetAttack(_monsterType);
                    gManager.ShowSystemMessage(_monsterType.ToString().ToUpper() + " used " + _monstersTurnAction.ToUpper() + "!");
                }
                else
                {
                    _timer = 0;
                    game.EndMonsterTurn(_monstersTurnAction);
                    _monstersTurnAction = "";
                }
            }
        }
    }
}
