using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Little_Might.Modules
{
    class WorldObject
    {
        private Texture2D _sprite;
        private Vector2 _position;
        private Color _color;
        private bool _fireAffected;

        public bool FireAffected
        {
            get { return _fireAffected; }
            set { _fireAffected = value; }
        }

        public Color ObjectColor
        {
            get { return _color; }
            set { _color = value; }
        }

        public Texture2D Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public WorldObject() { }

        public WorldObject(Texture2D sprite, Vector2 position, Color color)
        {
            _sprite = sprite;
            _position = position;
            _color = color;

            _fireAffected = false;
        }
    }
}
