using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Little_Might.Modules.Inventory;

namespace Little_Might.Utils
{
    class ItemInfo
    {
        public static Texture2D GetSpriteTexture(string itemName, ContentManager contentMgr)
        {
            switch (itemName.ToLower())
            {
                case "stonesword":
                    return contentMgr.Load<Texture2D>("stone_sword");
            }

            return null;
        }

        public static Texture2D GetSpriteTexture(ITEMTYPE type, ContentManager contentMgr)
        {
            switch (type)
            {
                case ITEMTYPE.FRUIT:
                    return contentMgr.Load<Texture2D>("icon_food");
                case ITEMTYPE.FLINT:
                    return contentMgr.Load<Texture2D>("flint");
                case ITEMTYPE.STONE:
                    return contentMgr.Load<Texture2D>("stone");
                case ITEMTYPE.STICK:
                    return contentMgr.Load<Texture2D>("twig");
                case ITEMTYPE.BERRY:
                    return contentMgr.Load<Texture2D>("berries");
                case ITEMTYPE.CAMPFIRE:
                    return contentMgr.Load<Texture2D>("campfire");
                case ITEMTYPE.TWINE:
                    return contentMgr.Load<Texture2D>("twine");
                case ITEMTYPE.THYME:
                    return contentMgr.Load<Texture2D>("thyme");
                case ITEMTYPE.GARLIC:
                    return contentMgr.Load<Texture2D>("garlic");
                case ITEMTYPE.OREGANO:
                    return contentMgr.Load<Texture2D>("oregano");
                case ITEMTYPE.GOOP:
                    return contentMgr.Load<Texture2D>("drops_slime");
                case ITEMTYPE.HARELEG:
                    return contentMgr.Load<Texture2D>("drop_hareleg");
                case ITEMTYPE.VEAL:
                    return contentMgr.Load<Texture2D>("drop_veal");
            }

            return null;
        }

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
                    return "A smooth, round\npiece of rock";
                case Modules.Inventory.ITEMTYPE.FLINT:
                    return "A sharp shard of\nblack flint";
                case Modules.Inventory.ITEMTYPE.CAMPFIRE:
                    return "Bundle of burning sticks";                
                case Modules.Inventory.ITEMTYPE.TWINE:
                    return "Strong piece of\nforest string";
                case Modules.Inventory.ITEMTYPE.THYME:
                    return "Tasty herb that\nfights infection";
                case Modules.Inventory.ITEMTYPE.GARLIC:
                    return "Strongly scented, good\n at fighting bacteria";
                case Modules.Inventory.ITEMTYPE.OREGANO:
                    return "A potent herb known\nto help inflamation";
                case Modules.Inventory.ITEMTYPE.GOOP:
                    return "Piece of a slime\nsticky and toxic";
                case Modules.Inventory.ITEMTYPE.VEAL:
                    return "Uncooked thick cut\nof deer meat";
                case Modules.Inventory.ITEMTYPE.HARELEG:
                    return "Uncooked leg of\n a wild rabbit";
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
                    return 20;
                case Modules.Inventory.ITEMTYPE.BERRY:
                    return 10;
                case Modules.Inventory.ITEMTYPE.GARLIC:
                    return 5;
                case Modules.Inventory.ITEMTYPE.OREGANO:
                    return 1;
                case Modules.Inventory.ITEMTYPE.THYME:
                    return 1;
                case Modules.Inventory.ITEMTYPE.VEAL:
                    return 50;
                case Modules.Inventory.ITEMTYPE.HARELEG:
                    return 35;
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
                return new Modules.InventoryItem(Utils.ItemInfo.GetSpriteTexture(Modules.Inventory.ITEMTYPE.CAMPFIRE, content), Vector2.Zero, 3f, Modules.Inventory.ITEMTYPE.CAMPFIRE);
            }

            if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.STONE, Modules.Inventory.ITEMTYPE.STICK, Modules.Inventory.ITEMTYPE.TWINE }))
            {
                return new Modules.InventoryItem(Utils.ItemInfo.GetSpriteTexture("stonesword", content), Vector2.Zero, 3f, Modules.Inventory.ITEMTYPE.WEAPON, "stonesword");
            }

            return null;
        }
    }
}
