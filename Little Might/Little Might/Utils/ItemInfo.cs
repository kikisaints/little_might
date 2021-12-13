using System;
using System.Collections.Generic;
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
                    return "A satiating fruit from\n a fruit tree.";
                case Modules.Inventory.ITEMTYPE.BERRY:
                    return "Small tasty morsel from\n a bush.";
                case Modules.Inventory.ITEMTYPE.TWIG:
                    return "Brittle and dry, it\nmight burn well.";
                case Modules.Inventory.ITEMTYPE.STONE:
                    return "A smooth, round pebble.";
                case Modules.Inventory.ITEMTYPE.FLINT:
                    return "A sharp shard of\nblack flint.";
                case Modules.Inventory.ITEMTYPE.COIN:
                    return "Shiny, some might\ncollect these.";
                default:
                    return "";
            }
        }
    }
}
