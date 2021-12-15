using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Little_Might.Utils
{
    class ItemInfo
    {
        public static string GetItemInformation(Modules.Inventory.ITEMTYPE itemType, string name = "")
        {
            switch(itemType)
            {
                case Modules.Inventory.ITEMTYPE.FRUIT:
                    return "A satiating fruit from\n a fruit tree";
                case Modules.Inventory.ITEMTYPE.BERRY:
                    return "Small tasty morsel from\n a bush";
                case Modules.Inventory.ITEMTYPE.STICK:
                    return "Brittle and dry, it\nmight burn well";
                case Modules.Inventory.ITEMTYPE.STONE:
                    return "A smooth, round pebble";
                case Modules.Inventory.ITEMTYPE.FLINT:
                    return "A sharp shard of\nblack flint";
                case Modules.Inventory.ITEMTYPE.COIN:
                    return "Shiny, some might\ncollect these";
                case Modules.Inventory.ITEMTYPE.CAMPFIRE:
                    return "Bundle of burning sticks";                
                case Modules.Inventory.ITEMTYPE.TWINE:
                    return "Strong piece of\nforest string";
                default:
                    break;
            }

            if (name != "")
            {
                if (name.ToLower() == "stonesword")
                    return "A blunt weapon\nmade of rock";
                if (name.ToLower() == "steelsword")
                    return "A shiny honed\nweapon of steel";
            }

            return "";
        }

        public static bool CheckIfPlacable(Modules.Inventory.ITEMTYPE itemType)
        {
            switch (itemType)
            {
                case Modules.Inventory.ITEMTYPE.CAMPFIRE:
                    return true;
                default:
                    return false;
            }
        }

        public static int GetItemHungerValue(Modules.Inventory.ITEMTYPE itemType)
        {
            switch (itemType)
            {
                case Modules.Inventory.ITEMTYPE.FRUIT:
                    return 35;
                case Modules.Inventory.ITEMTYPE.BERRY:
                    return 15;
                default:
                    return 0;
            }
        }

        public static int GetWeaponDamage(string name)
        {
            switch(name.ToLower())
            {
                case "stonesword":
                    return 5;
                case "steelsword":
                    return 25;
            }

            return 0;
        }

        public static Modules.InventoryItem GetItemByName(string name, ContentManager content)
        {
            if (name == "steelsword")
                return new Modules.InventoryItem(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("steel_sword"), Vector2.Zero, 3f, Modules.Inventory.ITEMTYPE.WEAPON, "steelsword");
            if (name == "stonesword")
                return new Modules.InventoryItem(content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("stone_sword"), Vector2.Zero, 3f, Modules.Inventory.ITEMTYPE.WEAPON, "stonesword");

            return null;
        }

        public static Modules.InventoryItem CheckCraftables(Modules.Inventory.ITEMTYPE[] itemCombo, Modules.Inventory inventory, ContentManager content)
        {
            itemCombo = itemCombo.OrderBy(e => ((int)e)).ToArray();

            if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.FLINT, Modules.Inventory.ITEMTYPE.STICK }))
            {
                return new Modules.InventoryItem(inventory.GetSpriteTexture(Modules.Inventory.ITEMTYPE.CAMPFIRE, content), Vector2.Zero, 3f, Modules.Inventory.ITEMTYPE.CAMPFIRE);
            }

            if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.STONE, Modules.Inventory.ITEMTYPE.STICK, Modules.Inventory.ITEMTYPE.TWINE }))
            {
                return new Modules.InventoryItem(inventory.GetSpriteTexture("stonesword", content), Vector2.Zero, 3f, Modules.Inventory.ITEMTYPE.WEAPON, "stonesword");
            }

            return null;
        }
    }
}
