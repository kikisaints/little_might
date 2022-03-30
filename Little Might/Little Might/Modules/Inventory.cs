using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;

namespace Little_Might.Modules
{
    class InventoryItem : ScreenObject
    {
        private Inventory.ITEMTYPE _itemType;
        private string _description;
        private string _name;
        private int _damage;
        private int _hunger;

        public bool Toggled = false;
        public bool IsPlaceable = false;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }

        public Inventory.ITEMTYPE ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        public int Hunger
        {
            get { return _hunger; }
            set { _hunger = value; }
        }

        public InventoryItem(InventoryItem copy, Vector2 position)
        {
            _itemType = copy.ItemType;
            _name = copy.Name;
            _description = copy.Description;
            _damage = copy.Damage;
            _hunger = copy.Hunger;
            Sprite = copy.Sprite;
            Scale = copy.Scale;
            IsPlaceable = copy.IsPlaceable;
            Position = position;
        }

        public InventoryItem() 
        {
            Scale = 3f;
        }
    }

    class Inventory
    {
        [Flags]
        public enum ITEMTYPE
        {
            NONE = 0,
            FRUIT,
            BERRY,
            FLINT,
            STONE,
            STICK,
            CAMPFIRE,
            WEAPON,
            TWINE,
            THYME,
            OREGANO,
            GARLIC,
            GOOP,
            HARELEG,
            VEAL,
            FURNACE,
            PELT,
            HEADGEAR,
            CHESTGEAR,
            FOOTGEAR,
            ACCESSORY
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

            Utils.ItemInfo.LoadAllItems(content);
        }

        public void RemoveSelectedItem()
        {
            _invItems.RemoveAt(_currentlySelectedIndex);
            RefreshInventory();
        }

        public void RemoveToggledItems()
        {
            List<InventoryItem> toRemove = new List<InventoryItem>();
            for (int i = 0; i < _invItems.Count; i++)
            {
                if (_invItems[i].Toggled)
                {
                    toRemove.Add(_invItems[i]);
                }
            }

            foreach (InventoryItem iItem in toRemove)
            {
                _invItems.Remove(iItem);
            }

            RefreshInventory();
        }

        public InventoryItem GetSelectedItem()
        {
            if (_invItems.Count > 0)
            {
                _currentlySelectedIndex = Utils.MathHandler.Get1DIndex(_invYIndex, _invXIndex, _invXSlots + 1);

                if (_currentlySelectedIndex < _invItems.Count)
                    return (_invItems[_currentlySelectedIndex]);
            }

            return null;
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
        public bool AddItem(ITEMTYPE type)
        {
            foreach (InventoryItem item in Utils.ItemInfo.AllItems)
            {
                if (item.ItemType == type)
                {
                    _invItems.Add(new InventoryItem(item, GetNewInvSlotPosition()));
                    _invCols++;

                    return true;
                }
            }

            return false;
        }

        public bool AddItem(string itemName)
        {
            foreach (InventoryItem item in Utils.ItemInfo.AllItems)
            {
                if (item.Name.ToLower() == itemName.ToLower())
                {
                    _invItems.Add(new InventoryItem(item, GetNewInvSlotPosition()));
                    _invCols++;

                    return true;
                }
            }

            return false;
        }

        public bool AddItem(InventoryItem invItem)
        {
            if (invItem != null && _invItems.Count + 1 < _maxInvSlots)
            {
                invItem.Position = GetNewInvSlotPosition();
                _invItems.Add(invItem);
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
    }
}
