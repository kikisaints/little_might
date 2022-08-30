using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Little_Might.Modules
{
    internal class ScreenObject
    {
        private Texture2D _sprite;
        private Texture2D _selectedSprite;
        private Vector2 _position;
        private float _scale;

        public Texture2D Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public Texture2D SelectedSprite
        {
            get { return _selectedSprite; }
            set { _selectedSprite = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public ScreenObject(Texture2D tex, Vector2 pos, float size)
        {
            _sprite = tex;
            _scale = size;
            _position = pos;
        }

        public ScreenObject()
        { }
    }
}
