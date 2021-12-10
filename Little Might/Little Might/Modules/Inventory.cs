using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Modules
{
    class InventoryItem : ScreenObject
    {
        public InventoryItem(Texture2D tex, Vector2 pos, float size)
        {
            Sprite = tex;
            Position = pos;
            Scale = size;
        }
    }

    class Inventory
    {
        public enum ITEMTYPE
        {
            NONE = 0,
            FRUIT,
            BERRY,
            FLINT,
            STONE,
            TWIG,
            COIN
        }        

        private List<InventoryItem> _invItems;
        private Vector2 _startingSlotPos;
        private Vector2 _spacing;
        private int _maxInvSlots = 78;
        private int _invRows = 0;
        private int _invCols = 0;

        public List<InventoryItem> Items
        {
            get { return _invItems; }
        }

        public Inventory()
        {
            _invItems = new List<InventoryItem>();
            _startingSlotPos = new Vector2(-280, 235);
            _spacing = new Vector2(35, 35);
        }

        public void AddItem(ITEMTYPE type, ContentManager content)
        {
            Texture2D sprite = GetSpriteTexture(type, content);

            if (sprite != null && _invItems.Count + 1 < _maxInvSlots)
            {
                _invItems.Add(new InventoryItem(sprite, GetInvSlotPosition(), 3f));
                _invCols++;
            }
        }

        private Vector2 GetInvSlotPosition()
        {
            if (_invItems.Count % 7 == 0 && _invItems.Count > 1)
            {
                _invRows++;
                _invCols = 0;
            }

            return new Vector2(_startingSlotPos.X + (_spacing.X * _invCols), _startingSlotPos.Y + (_spacing.Y * _invRows));
        }

        private Texture2D GetSpriteTexture(ITEMTYPE type, ContentManager contentMgr)
        {
            switch(type)
            {
                case ITEMTYPE.FRUIT:
                    return contentMgr.Load<Texture2D>("icon_food");
                case ITEMTYPE.FLINT:
                    return contentMgr.Load<Texture2D>("flint");
                case ITEMTYPE.STONE:
                    return contentMgr.Load<Texture2D>("stone");
                case ITEMTYPE.TWIG:
                    return contentMgr.Load<Texture2D>("twig");
                case ITEMTYPE.BERRY:
                    return contentMgr.Load<Texture2D>("berries");
            }

            return null;
        }
    }
}
