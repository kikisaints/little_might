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
        Inventory.ITEMTYPE _itemType;

        public Inventory.ITEMTYPE Type
        {
            get { return _itemType; }
        }

        public InventoryItem(Texture2D tex, Vector2 pos, float size, Inventory.ITEMTYPE invType)
        {
            Sprite = tex;
            Position = pos;
            Scale = size;
            _itemType = invType;
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
        private Vector2 _startingInvSelectorPos;
        private Vector2 _spacing;
        private int _maxInvSlots = 78;

        private int _invXSlots = 6;
        private int _invYSlots = 10;
        private int _invXIndex = 0;
        private int _invYIndex = 0;

        private int _invRows = 0;
        private int _invCols = 0;
        private int _currentlySelectedIndex = 0;
        private ScreenObject _invSelector;

        public bool NavigatingInventory = false;

        public ScreenObject InventorySelector
        {
            get { return _invSelector; }
        }

        public List<InventoryItem> Items
        {
            get { return _invItems; }
        }

        public Inventory(ContentManager content)
        {
            _invItems = new List<InventoryItem>();
            _startingSlotPos = new Vector2(-282, 233);
            _startingInvSelectorPos = new Vector2(-288, 227);
            _spacing = new Vector2(35, 35);
            _invSelector = new ScreenObject(content.Load<Texture2D>("inventory_selector"), 
                _startingInvSelectorPos, 
                4f);
        }

        public void RemoveSelectedItem()
        {
            _invItems.RemoveAt(_currentlySelectedIndex);
            RefreshInventory();
        }

        public ITEMTYPE GetSelectedItem()
        {
            if (_invItems.Count > 0)
            {
                _currentlySelectedIndex = Utils.ArrayHandler.Get1DIndex(_invYIndex, _invXIndex, _invXSlots + 1);

                if (_currentlySelectedIndex < _invItems.Count)
                    return (_invItems[_currentlySelectedIndex].Type);
            }

            return ITEMTYPE.NONE;
        }

        public void UpdateInventory(Utils.InputManager inputManager)
        {
            if (NavigatingInventory)
            {
                if (inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Left) && _invXIndex > 0)
                {
                    _invSelector.Position -= new Vector2(_spacing.X, 0);
                    _invXIndex--;
                }
                if (inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Right) && _invXIndex < _invXSlots)
                {
                    _invSelector.Position += new Vector2(_spacing.X, 0);
                    _invXIndex++;
                }
                if (inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Up) && _invYIndex > 0)
                {
                    _invSelector.Position -= new Vector2(0, _spacing.Y);
                    _invYIndex--;
                }
                if (inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Down) && _invYIndex < _invYSlots)
                {
                    _invSelector.Position += new Vector2(0, _spacing.Y);
                    _invYIndex++;
                }
            }
        }

        //Returns true if the item was successfully added to the inventory
        public bool AddItem(ITEMTYPE type, ContentManager content)
        {
            Texture2D sprite = GetSpriteTexture(type, content);

            if (sprite != null && _invItems.Count + 1 < _maxInvSlots)
            {
                _invItems.Add(new InventoryItem(sprite, GetNewInvSlotPosition(), 3f, type));
                _invCols++;

                return true;
            }

            return false;
        }

        public void RefreshInventory()
        {
            _invRows = 0;
            _invCols = 0;

            for (int i = 0; i < _invItems.Count; i++)
            {
                _invItems[i].Position = GetNewInvSlotPosition();
                _invCols++;
            }
        }

        private Vector2 GetNewInvSlotPosition()
        {
            if (_invCols % 7 == 0 && _invItems.Count > 1 && _invCols != 0)
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
