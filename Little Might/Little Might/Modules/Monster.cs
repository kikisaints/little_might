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
            SLIME = 0,
            DEER,
            RABBIT,
            CELESTIALHORROR
        }

        public static Inventory.ITEMTYPE GetDrop(MONSTERTYPE type)
        {
            switch(type.ToString().ToLower())
            {
                case "slime":
                    return Inventory.ITEMTYPE.GOOP;
                case "rabbit":
                    return Inventory.ITEMTYPE.HARELEG;
                case "deer":
                    return Inventory.ITEMTYPE.VEAL;
            }

            return Inventory.ITEMTYPE.NONE;
        }

        private Stats _stats;
        private Texture2D _interactionSprite;
        private MONSTERTYPE _monsterType;
        private string _name;
        private Color _monsterColor;
        private string _itemDrop;
        private string[] _attacks;

        private double _timer = 0;
        private double _turnWaitTime = 1.5;
        private string _monstersTurnAction = "";
        private Random _movementWaitTime = new Random();

        public double MoveTime = 0;

        public string[] Attacks
        {
            get { return _attacks; }
            set { _attacks = value; }
        }

        public string ItemDrop
        {
            get { return _itemDrop; }
            set { _itemDrop = value; }
        }

        public Color MonsterColor
        {
            get { return _monsterColor; }
            set { _monsterColor = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Stats Stats
        {
            get { return _stats; }
            set { _stats = value; }
        }

        public Texture2D InteractionSprite
        {
            get { return _interactionSprite; }
            set { _interactionSprite = value; }
        }

        public MONSTERTYPE MonsterType
        {
            get { return _monsterType; }
            set { _monsterType = value; }
        }

        public Monster() 
        {
            MoveTime = _movementWaitTime.Next(1, 5) * 0.5;
        }

        public Monster(string spriteName, string intractSpriteName, Vector2 startingPosition, ContentManager contentManager, Color color, MONSTERTYPE type, Stats monsterStats)
        {
            _stats = monsterStats;
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

                if ((_monsterType == MONSTERTYPE.SLIME || _monsterType == MONSTERTYPE.RABBIT) && 
                    map.GetTileType(movePosition) == Utils.WorldMap.MAPTILETYPE.GRASS &&
                    map.GetTileType(movePosition) != Utils.WorldMap.MAPTILETYPE.OUTOFBOUNDS)
                    Position = movePosition;
                else if ((_monsterType == MONSTERTYPE.DEER) && 
                    map.GetTileType(movePosition) != Utils.WorldMap.MAPTILETYPE.EVERGREEN &&
                    map.GetTileType(movePosition) != Utils.WorldMap.MAPTILETYPE.WATER &&
                    map.GetTileType(movePosition) != Utils.WorldMap.MAPTILETYPE.OUTOFBOUNDS)
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

                    if (_monsterType.ToString().ToUpper() != "CELESTIALHORROR")
                        gManager.ShowSystemMessage(_monsterType.ToString().ToUpper() + " used " + _monstersTurnAction.ToUpper() + "!");
                    else
                        gManager.ShowSystemMessage("??? used ????!");
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
