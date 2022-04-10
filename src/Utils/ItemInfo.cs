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
    internal class ItemInfo
    {
        public static List<Modules.InventoryItem> AllItems;

        public static void LoadAllItems(ContentManager content)
        {
            AllItems = new List<Modules.InventoryItem>();

            XmlDocument allItemsList = new XmlDocument();
            allItemsList.Load(@"..\..\..\Content\data\ItemData.xml");

            XmlNodeList nodes = allItemsList.SelectNodes("//Items/Item");

            foreach (XmlNode node in nodes)
            {
                InventoryItem item = new InventoryItem();

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    string itemAttribute = childNode.Name;
                    ITEMTYPE checkType;

                    if (itemAttribute == "Name")
                        item.Name = childNode.InnerText;
                    else if (itemAttribute == "ItemType")
                    {
                        if (Enum.TryParse(childNode.InnerText.ToUpper(), out checkType))
                            item.ItemType = checkType;
                    }
                    else if (itemAttribute == "Sprite")
                        item.Sprite = content.Load<Texture2D>($"images/{childNode.InnerText}");
                    else if (itemAttribute == "Hunger")
                        item.Hunger = Int32.Parse(childNode.InnerText);
                    else if (itemAttribute == "Description")
                        item.Description = Regex.Unescape(childNode.InnerText);
                    else if (itemAttribute == "Damage")
                        item.Damage = Int32.Parse(childNode.InnerText);
                    else if (itemAttribute == "Placable")
                        if (childNode.InnerText == "yes")
                            item.IsPlaceable = true;
                }

                AllItems.Add(item);
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

        //Tthis list is -- ORDERED --, meaning that the if checks must check for the order of largest enum value to smallest.
        // It doesn't matter the order the player has selected for crafting pieces, only the final if check order
        public static bool CheckCraftables(Modules.Inventory.ITEMTYPE[] itemCombo, ref Modules.Inventory inventory, GraphicsManager graphicsManager)
        {
            itemCombo = itemCombo.OrderBy(e => ((int)e)).ToArray();

            if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] {
                Modules.Inventory.ITEMTYPE.NONE,
                Modules.Inventory.ITEMTYPE.NONE,
                Modules.Inventory.ITEMTYPE.FLINT,
                Modules.Inventory.ITEMTYPE.STICK }))
            {
                inventory.AddItem(ITEMTYPE.CAMPFIRE);
                graphicsManager.ShowSystemMessage("Made " + ITEMTYPE.CAMPFIRE.ToString().ToUpper() + "!");
                return true;
            }
            else if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] {
                Modules.Inventory.ITEMTYPE.NONE,
                Modules.Inventory.ITEMTYPE.STONE,
                Modules.Inventory.ITEMTYPE.STICK,
                Modules.Inventory.ITEMTYPE.TWINE }))
            {
                inventory.AddItem("stone sword");
                graphicsManager.ShowSystemMessage("Made STONE SWORD!");
                return true;
            }
            else if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] {
                Modules.Inventory.ITEMTYPE.STONE,
                Modules.Inventory.ITEMTYPE.STONE,
                Modules.Inventory.ITEMTYPE.CAMPFIRE,
                Modules.Inventory.ITEMTYPE.GOOP }))
            {
                inventory.AddItem("furnace");
                graphicsManager.ShowSystemMessage("Made FURNACE!");
                return true;
            }
            else if (itemCombo.SequenceEqual(new Modules.Inventory.ITEMTYPE[] {
                Modules.Inventory.ITEMTYPE.NONE,
                Modules.Inventory.ITEMTYPE.NONE,
                Modules.Inventory.ITEMTYPE.FLINT,
                Modules.Inventory.ITEMTYPE.PELT }))
            {
                inventory.AddItem("leather cap");
                inventory.AddItem("leather shirt");
                inventory.AddItem("leather shoes");
                graphicsManager.ShowSystemMessage("Made LEATHER SET!");
                return true;
            }

            return false;
        }
    }
}
