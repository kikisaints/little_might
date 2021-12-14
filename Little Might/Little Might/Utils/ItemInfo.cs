using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Little_Might.Utils
{
    class ItemInfo
    {
        public static string GetItemInformation(Modules.Inventory.ITEMTYPE itemType)
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
                case Modules.Inventory.ITEMTYPE.STONESWORD:
                    return "Rather blunt weapon\nmade from rock";
                case Modules.Inventory.ITEMTYPE.TWINE:
                    return "Strong piece of forest string";
                default:
                    return "";
            }
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

        public static Modules.Inventory.ITEMTYPE CheckCraftables(Modules.Inventory.ITEMTYPE[] itemCombo)
        {
            itemCombo = itemCombo.OrderBy(e => ((int)e)).ToArray();

            if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.FLINT, Modules.Inventory.ITEMTYPE.STICK }))
            {
                return Modules.Inventory.ITEMTYPE.CAMPFIRE;
            }

            if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.STONE, Modules.Inventory.ITEMTYPE.STICK, Modules.Inventory.ITEMTYPE.TWINE }))
            {
                return Modules.Inventory.ITEMTYPE.STONESWORD;
            }

            return Modules.Inventory.ITEMTYPE.NONE;
        }
    }
}
