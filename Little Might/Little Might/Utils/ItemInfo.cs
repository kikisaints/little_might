using Little_Might.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using static Little_Might.Modules.Inventory;

namespace Little_Might.Utils
{
    class ItemInfo
    {
        public static List<Modules.InventoryItem> AllItems;

        public static void LoadAllItems(ContentManager content)
        {
            AllItems = new List<Modules.InventoryItem>();

            XmlDocument allItemsList = new XmlDocument();
            allItemsList.Load(@"..\netcoreapp3.1\Content\data\ItemData.xml");

            XmlNodeList nodes = allItemsList.SelectNodes("//Items/Item");

            foreach (XmlNode node in nodes)
            {
                InventoryItem item = new InventoryItem();

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    string itemAttribute = childNode.Name;

                    if (itemAttribute == "Name")
                        item.Name = childNode.InnerText;
                    else if (itemAttribute == "ItemType")
                        item.ItemType = Utils.ItemInfo.GetTypeFromString(childNode.InnerText);
                    else if (itemAttribute == "Sprite")
                        item.Sprite = content.Load<Texture2D>(childNode.InnerText);
                    else if (itemAttribute == "Hunger")
                        item.Hunger = Int32.Parse(childNode.InnerText);
                    else if (itemAttribute == "Description")
                        item.Description = Regex.Unescape(childNode.InnerText);
                    else if (itemAttribute == "Damage")
                        item.Damage = Int32.Parse(childNode.InnerText);
                }

                item.IsPlaceable = CheckIfPlacable(item.ItemType);
                AllItems.Add(item);
            }
        }

        public static Modules.Inventory.ITEMTYPE GetTypeFromString(string type)
        {
            switch (type)
            {
                case "fruit":
                    return ITEMTYPE.FRUIT;
                case "berry":
                    return ITEMTYPE.BERRY;
                case "stick":
                    return ITEMTYPE.STICK;
                case "stone":
                    return ITEMTYPE.STONE;
                case "flint":
                    return ITEMTYPE.FLINT;
                case "campfire":
                    return ITEMTYPE.CAMPFIRE;
                case "twine":
                    return ITEMTYPE.TWINE;
                case "thyme":
                    return ITEMTYPE.THYME;
                case "garlic":
                    return ITEMTYPE.GARLIC;
                case "oregano":
                    return ITEMTYPE.OREGANO;
                case "goop":
                    return ITEMTYPE.GOOP;
                case "hareleg":
                    return ITEMTYPE.HARELEG;
                case "weapon":
                    return ITEMTYPE.WEAPON;
                case "veal":
                    return ITEMTYPE.VEAL;
                case "furnace":
                    return ITEMTYPE.FURNACE;
                default:
                    return ITEMTYPE.NONE;
            }
        }

        public static Texture2D GetSpriteTexture(ITEMTYPE type)
        {
            foreach (InventoryItem item in AllItems)
            {
                if (item.ItemType == type)
                    return item.Sprite;
            }

            return null;
        }

        public static InventoryItem GetItemFromString(string name)
        {
            foreach (InventoryItem item in AllItems)
            {
                if (item.Name.ToLower() == name.ToLower())
                    return item;
            }

            return null;
        }

        public static bool CheckIfPlacable(Modules.Inventory.ITEMTYPE itemType)
        {
            switch (itemType)
            {
                case Modules.Inventory.ITEMTYPE.CAMPFIRE:
                    return true;
                case Modules.Inventory.ITEMTYPE.FURNACE:
                    return true;
                default:
                    return false;
            }
        }

        //Tthis list is -- ORDERED --, meaning that the if checks must check for the order of largest enum value to smallest.
        // It doesn't matter the order the player has selected for crafting pieces, only the final if check order
        public static bool CheckCraftables(Modules.Inventory.ITEMTYPE[] itemCombo, ref Modules.Inventory inventory, GraphicsManager graphicsManager)
        {
            itemCombo = itemCombo.OrderBy(e => ((int)e)).ToArray();

            if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.FLINT, Modules.Inventory.ITEMTYPE.STICK }))
            {
                inventory.AddItem(ITEMTYPE.CAMPFIRE);
                graphicsManager.ShowSystemMessage("Made " + ITEMTYPE.CAMPFIRE.ToString().ToUpper() + "!");
                return true;
            }
            else if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.NONE, Modules.Inventory.ITEMTYPE.STONE, Modules.Inventory.ITEMTYPE.STICK, Modules.Inventory.ITEMTYPE.TWINE }))
            {
                inventory.AddItem("stone sword");
                graphicsManager.ShowSystemMessage("Made STONE SWORD!");
                return true;
            }
            else if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] { Modules.Inventory.ITEMTYPE.STONE, Modules.Inventory.ITEMTYPE.STONE, Modules.Inventory.ITEMTYPE.CAMPFIRE, Modules.Inventory.ITEMTYPE.GOOP }))
            {
                inventory.AddItem("furnace");
                graphicsManager.ShowSystemMessage("Made FURNACE!");
                return true;
            }

            return false;
        }
    }
}
