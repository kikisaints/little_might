using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Little_Might.Modules
{
    class Character : WorldObject
    {
        private AdvancedStats _stats;
        private Utils.InputManager _inputManager;
        private Inventory _playerInventory;
        private Utils.GraphicsManager _graphicsManager;
        private Game1 _game;

        private Texture2D _interactionSprite;
        private int _steps = 0;
        private bool _canMove = true;
        private bool _fighting = false;
        private int _fieldOfView = 10;

        private string[] _currentOptions;
        private string[] _interactionOptions;
        private string[] _fightOptions;
        private int _interactIndex = 0;

        private string[] _equippedItems;
        private Texture2D _weaponSprite = null;
        private Texture2D _headgearSprite = null;
        private Texture2D _chestgearSprite = null;
        private Texture2D _footgearSprite = null;
        private Texture2D _accessorySprite = null;

        private Modules.InventoryItem _equippedWeapon = null;

        private int _craftIndex = 0;
        private Modules.Inventory.ITEMTYPE[] _craftingList = {
            Modules.Inventory.ITEMTYPE.NONE,
            Modules.Inventory.ITEMTYPE.NONE,
            Modules.Inventory.ITEMTYPE.NONE,
            Modules.Inventory.ITEMTYPE.NONE
        };

        public int FieldOfView
        {
            get { return _fieldOfView; }
        }

        public Inventory Inv
        {
            get { return _playerInventory; }
        }

        public AdvancedStats Stats
        {
            get { return _stats; }
        }

        public Texture2D InteractionSprite
        {
            get { return _interactionSprite; }
        }

        public string[] InteractionOptions
        {
            get { return _currentOptions; }
        }

        public string[] EquippedItems
        {
            get { return _equippedItems; }
        }

        public Modules.InventoryItem GetWeapon() { return _equippedWeapon; }

        public Texture2D GetWeaponSprite()
        {
            if (_equippedItems[0] == "NO WEAPON")
                return _playerInventory.InventorySelector.Sprite;
            else
                return _weaponSprite;
        }

        public Texture2D GetHeadgearSprite()
        {
            if (_equippedItems[1] == "NO HEADGEAR")
                return _playerInventory.InventorySelector.Sprite;
            else
                return _headgearSprite;
        }

        public Texture2D GetChestgearSprite()
        {
            if (_equippedItems[3] == "NO CHESTGEAR")
                return _playerInventory.InventorySelector.Sprite;
            else
                return _chestgearSprite;
        }

        public Texture2D GetFootgearSprite()
        {
            if (_equippedItems[2] == "NO FOOTGEAR")
                return _playerInventory.InventorySelector.Sprite;
            else
                return _footgearSprite;
        }

        public Texture2D GetAccessorySprite()
        {
            if (_equippedItems[4] == "NO ACCESSORY")
                return _playerInventory.InventorySelector.Sprite;
            else
                return _accessorySprite;
        }

        public Character (string spriteName, string interactSpriteName, Vector2 startingPosition, ContentManager contentManager, Utils.GraphicsManager gManager, Game1 gameClass)
        {
            _inputManager = new Utils.InputManager();
            _stats = new AdvancedStats(50, 10, 0, 1, 10, 10, 10, 10, 10, 2f, 0f);
            _playerInventory = new Inventory(contentManager);
            _interactionOptions = new string[] { "Communicate -", "Fight", "Run Away" };
            _fightOptions = Utils.CombatHandler.SetCombatOptions("none");
            _equippedItems = new string[] { "NO WEAPON", "NO HEADGEAR", "NO FOOTGEAR", "NO CHESTGEAR", "NO ACCESSORY" };

            _currentOptions = _interactionOptions;

            _game = gameClass;
            _interactionSprite = contentManager.Load<Texture2D>(interactSpriteName);
            Sprite = contentManager.Load<Texture2D>(spriteName);
            Position = startingPosition;
            ObjectColor = Color.White;
            _graphicsManager = gManager;
        }

        public void UpdateCharacter(GameTime time, Utils.WorldMap map, ContentManager content, bool inInteraction = false)
        {
            _inputManager.UpdateInput(time);

            if (!inInteraction)
            {
                UpdateMovement(map);                
                UpdateInventoryInteraction(content, map);                

                if (_canMove)
                {
                    UpdateInput(map, content);

                    if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Enter))
                    {
                        if (WaterTileNearby(map))
                        {
                            //run the Cholera chance here...
                            _stats.Hydration += 25;
                            _graphicsManager.ShowSystemMessage("+25 HYDRATION");

                            if (Utils.MathHandler.GetRandomNumber(0, 10) == 10)
                                _stats.Illness = AdvancedStats.ILLNESSES.CHOLERA;
                        }
                    }
                }                
            }
            else
            {
                _inputManager.MoveSpeed = 0.15;
                if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    if (_interactIndex + 1 < _currentOptions.Length)
                    {
                        _currentOptions[_interactIndex] = _currentOptions[_interactIndex].Remove(_currentOptions[_interactIndex].Length - 2, 2);
                        _interactIndex++;
                        _currentOptions[_interactIndex] += " -";
                    }
                }
                else if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    if (_interactIndex - 1 >= 0)
                    {
                        _currentOptions[_interactIndex] = _currentOptions[_interactIndex].Remove(_currentOptions[_interactIndex].Length - 2, 2);
                        _interactIndex--;
                        _currentOptions[_interactIndex] += " -";
                    }
                }

                if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    if (_currentOptions[_interactIndex] == "Run Away -")
                    {
                        _currentOptions[_interactIndex] = _currentOptions[_interactIndex].Remove(_currentOptions[_interactIndex].Length - 2, 2);
                        _interactIndex = 0;
                        _currentOptions[_interactIndex] += " -";
                        _game.LeaveInteraction();

                        _graphicsManager.ShowSystemMessage("You ran away!");
                    }
                    else if (_currentOptions[_interactIndex] == "Fight -")
                    {
                        _currentOptions[_interactIndex] = _currentOptions[_interactIndex].Remove(_currentOptions[_interactIndex].Length - 2, 2);
                        _currentOptions[0] += " -";
                        _currentOptions = _fightOptions;
                        _interactIndex = 0;
                        _graphicsManager.ShowSystemMessage("You Choose to Fight");
                        _fighting = true;
                    }
                    else if (_fighting)
                    {
                        _game.EndPlayerTurn(_currentOptions[_interactIndex].Remove(_currentOptions[_interactIndex].Length - 2, 2));
                    }
                }
            }
        }

        public void EndFight()
        {
            _currentOptions[_interactIndex] = _currentOptions[_interactIndex].Remove(_currentOptions[_interactIndex].Length - 2, 2);
            _interactIndex = 0;
            _currentOptions[_interactIndex] += " -";
            _currentOptions = _interactionOptions;
        }

        private void CheckTileResource(Vector2 movePos, Utils.WorldMap wMap, ContentManager content)
        {
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.FRUIT)
            {
                if (_playerInventory.AddItem(Inventory.ITEMTYPE.FRUIT))
                    _graphicsManager.ShowSystemMessage("Picked up " + Inventory.ITEMTYPE.FRUIT.ToString());
            }
            else if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.BUSH)
            {
                Inventory.ITEMTYPE randomBushItem = wMap.GetBushItem();

                if (randomBushItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomBushItem))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomBushItem.ToString());
                }
            }
            else if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.HERBS)
            {
                Inventory.ITEMTYPE randomHerbItem = wMap.GetHerbItem();

                if (randomHerbItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomHerbItem))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomHerbItem.ToString());
                }
            }
            else if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.ROCK)
            {
                Inventory.ITEMTYPE randomRockItem = wMap.GetStoneItem();

                if (randomRockItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomRockItem))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomRockItem.ToString());
                }
            }
            else if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.TREE)
            {
                Inventory.ITEMTYPE randomTreeItem = wMap.GetTreeItem();

                if (randomTreeItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomTreeItem))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomTreeItem.ToString());
                }
            }
            else if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.ITEMDROP)
            {
                WorldObject obj = _graphicsManager.GetWorldObject((int)movePos.X, (int)movePos.Y);
                Inventory.ITEMTYPE monsterDropType = Utils.MonsterManager.GetMonsterDrop(obj.ObjectColor);

                if (_playerInventory.AddItem(monsterDropType))
                    _graphicsManager.ShowSystemMessage("Picked up " + monsterDropType.ToString());

                wMap.ChangeTile(new Vector2(Position.X, Position.Y), Utils.WorldMap.MAPTILETYPE.GRASS);
                _graphicsManager.ChangeWorldObjectVisual(content.Load<Texture2D>("tile_grass"),
                    Utils.GameColors.GrassMapColor,
                    (int)Position.X,
                    (int)Position.Y,
                    Modules.Inventory.ITEMTYPE.NONE);
            }
            else if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.CHEST)
            {
                Inventory.ITEMTYPE randomChestItem = wMap.GetChestItem();

                if (randomChestItem == Inventory.ITEMTYPE.WEAPON)
                {
                    int chance = Utils.MathHandler.GetRandomNumber(0, 1);
                    if (chance == 0)
                    {
                        if (_playerInventory.AddItem("steel sword"))
                            _graphicsManager.ShowSystemMessage("Picked up STEELSWORD");                        
                    }
                    else if (chance == 1)
                    {
                        if (_playerInventory.AddItem("stone sword"))
                            _graphicsManager.ShowSystemMessage("Picked up STONESWORD");
                    }
                }
                else
                {
                    if (_playerInventory.AddItem(randomChestItem))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomChestItem.ToString());
                }

                _graphicsManager.ChangeWorldObjectVisual(content.Load<Texture2D>("tile_grass"),
                    Utils.GameColors.GrassMapColor,
                    (int)Position.X,
                    (int)Position.Y,
                    Modules.Inventory.ITEMTYPE.NONE,
                    Utils.WorldMap.MAPTILETYPE.GRASS);
            }
            else if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.PRARIEDUNGEON)
            {
                Utils.WorldMap.Teleportal portal = wMap.GetPortalByLocation(movePos);

                //if the portal has a name, than it's legit, therefore we can teleport to it's other location
                if (portal.PortalName != "")
                {
                    if (DrawLayer != portal.PortalEnterLayer)
                    {
                        _graphicsManager.ShowSystemMessage("Entered Dungeon\n" + portal.PortalName.ToUpper() + "!");
                        _graphicsManager.SetDrawLayer(portal.PortalEnterLayer);
                        DrawLayer = portal.PortalEnterLayer;
                    }
                    else
                    {
                        _graphicsManager.ShowSystemMessage("Exited Dungeon\n" + portal.PortalName.ToUpper() + "!");
                        _graphicsManager.SetDrawLayer(0);
                        DrawLayer = 0;
                    }
                }
            }

            if (_graphicsManager.IsCampfireAffectedTile((int)movePos.X, (int)movePos.Y))
            {
                if (_stats.HP < _stats.BaseHP)
                    _stats.HP += 1;

                if (_stats.HP > _stats.BaseHP)
                    _stats.HP = _stats.BaseHP;
            }
        }

        private bool WaterTileNearby(Utils.WorldMap map)
        {
            if (map.GetTileType(new Vector2(Position.X + Utils.WorldMap.UNITSIZE, Position.Y)) == Utils.WorldMap.MAPTILETYPE.WATER ||
                map.GetTileType(new Vector2(Position.X - Utils.WorldMap.UNITSIZE, Position.Y)) == Utils.WorldMap.MAPTILETYPE.WATER ||
                map.GetTileType(new Vector2(Position.X, Position.Y + Utils.WorldMap.UNITSIZE)) == Utils.WorldMap.MAPTILETYPE.WATER ||
                map.GetTileType(new Vector2(Position.X, Position.Y - Utils.WorldMap.UNITSIZE)) == Utils.WorldMap.MAPTILETYPE.WATER)
                return true;
            return false;
        }

        private void UpdateInventoryInteraction(ContentManager content, Utils.WorldMap map)
        {
            if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.I))
            {
                _canMove = !_canMove;
                _playerInventory.NavigatingInventory = !_playerInventory.NavigatingInventory;

                _craftIndex = 0;
                Array.Clear(_craftingList, 0, _craftingList.Length);
            }

            if (_playerInventory.NavigatingInventory)
            {
                _playerInventory.UpdateInventory(_inputManager);
            }

            if (_playerInventory.NavigatingInventory)
            {
                if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.T))
                {
                    if (_playerInventory.GetSelectedItem() == null)
                        return;

                    if (_equippedItems[0].ToLower() != _playerInventory.GetSelectedItem().Name.ToLower())
                    {
                        _graphicsManager.ShowSystemMessage("Trashed " + _playerInventory.GetSelectedItem().Name.ToUpper() + "!");
                        _playerInventory.RemoveSelectedItem();
                    }
                    else
                    {
                        _graphicsManager.ShowSystemMessage("Can't trash item!");
                    }
                }

                if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    if (_playerInventory.GetSelectedItem() == null)
                        return;

                    Inventory.ITEMTYPE _itemType = _playerInventory.GetSelectedItem().ItemType;                    

                    if (_itemType == Inventory.ITEMTYPE.WEAPON)
                    {
                        EquipItem(ref _weaponSprite, 0, _itemType);
                        return;
                    }
                    else if (_itemType == Inventory.ITEMTYPE.HEADGEAR)
                    {
                        EquipItem(ref _headgearSprite, 1, _itemType);
                        return;
                    }
                    else if (_itemType == Inventory.ITEMTYPE.CHESTGEAR)
                    {
                        EquipItem(ref _chestgearSprite, 3, _itemType);
                        return;
                    }
                    else if (_itemType == Inventory.ITEMTYPE.FOOTGEAR)
                    {
                        EquipItem(ref _footgearSprite, 2, _itemType);
                        return;
                    }
                    else if (_itemType == Inventory.ITEMTYPE.ACCESSORY)
                    {
                        EquipItem(ref _accessorySprite, 4, _itemType);
                        return;
                    }

                    int _hungerValue = _playerInventory.GetSelectedItem().Hunger;
                    _playerInventory.GetSelectedItem().Toggled = !_playerInventory.GetSelectedItem().Toggled;

                    if (_hungerValue != 0)
                    {
                        SetIngestedIllness(_playerInventory.GetSelectedItem().ItemType);

                        _stats.Hunger += _hungerValue;

                        if (_playerInventory.GetSelectedItem().ItemType == Inventory.ITEMTYPE.FRUIT)
                        {
                            _stats.Hydration += (int)(_hungerValue / 4);

                            _graphicsManager.ShowSystemMessage("+" + _hungerValue.ToString() + " HUNGER and\n+" + ((int)(_hungerValue / 4)).ToString() + " THIRST");
                            _playerInventory.RemoveSelectedItem();
                        }
                        else
                        {
                            _graphicsManager.ShowSystemMessage("+" + _hungerValue.ToString() + " HUNGER");
                            _playerInventory.RemoveSelectedItem();
                        }

                        return;
                    }
                    else if (_craftIndex < 4 && _playerInventory.GetSelectedItem().Toggled)
                    {
                        _craftingList[_craftIndex] = _itemType;
                        _craftIndex++;
                    }
                    else if (!_playerInventory.GetSelectedItem().Toggled)
                    {
                        _craftIndex--;
                        for (int i = 0; i < _craftingList.Length; i++)
                        {
                            if (_craftingList[i] == _itemType)
                            {
                                _craftingList[i] = Inventory.ITEMTYPE.NONE;
                                break;
                            }
                        }

                        if (_craftIndex < 0)
                            _craftIndex = 0;
                    }


                    if (Utils.ItemInfo.CheckCraftables(_craftingList, ref _playerInventory, _graphicsManager))
                    {
                        _playerInventory.RemoveToggledItems();                        

                        _craftIndex = 0;
                        Array.Clear(_craftingList, 0, _craftingList.Length);
                    }
                }

                if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.P))
                {
                    if (_playerInventory.GetSelectedItem() == null)
                        return;

                    Inventory.ITEMTYPE _itemType = _playerInventory.GetSelectedItem().ItemType;

                    if (_playerInventory.GetSelectedItem().IsPlaceable)
                    {
                        if (_itemType == Inventory.ITEMTYPE.CAMPFIRE)
                        {
                            map.ChangeTile(new Vector2(Position.X, Position.Y + Utils.WorldMap.UNITSIZE), Utils.WorldMap.MAPTILETYPE.CAMPFIRE);
                            _graphicsManager.ChangeWorldObjectVisual(_playerInventory.GetSelectedItem().Sprite,
                                Utils.GameColors.CampfireMapColor,
                                (int)Position.X,
                                (int)Position.Y + Utils.WorldMap.UNITSIZE,
                                _itemType);
                        }
                        else if (_itemType == Inventory.ITEMTYPE.FURNACE)
                        {
                            map.ChangeTile(new Vector2(Position.X, Position.Y + Utils.WorldMap.UNITSIZE), Utils.WorldMap.MAPTILETYPE.FURNACE);
                            _graphicsManager.ChangeWorldObjectVisual(_playerInventory.GetSelectedItem().Sprite,
                                Utils.GameColors.FurnaceMapColor,
                                (int)Position.X,
                                (int)Position.Y + Utils.WorldMap.UNITSIZE,
                                _itemType);
                        }

                        _graphicsManager.ShowSystemMessage(_itemType.ToString().ToUpper() + " placed!");
                        _playerInventory.RemoveSelectedItem();

                        _craftIndex = 0;
                        Array.Clear(_craftingList, 0, _craftingList.Length);
                    }
                }
            }
        }

        public int GetDamageDefenseModifier()
        {
            int headgearDef = 0;
            int footgearDef = 0;
            int chestgearDef = 0;
            int accessoryDef = 0;

            if (_equippedItems[1] != "NO HEADGEAR")
                headgearDef = Utils.ItemInfo.GetItemFromString(_equippedItems[1]).Damage;
            if (_equippedItems[2] != "NO FOOTGEAR")
                footgearDef = Utils.ItemInfo.GetItemFromString(_equippedItems[2]).Damage;
            if (_equippedItems[3] != "NO CHESTGEAR")
                chestgearDef = Utils.ItemInfo.GetItemFromString(_equippedItems[3]).Damage;
            if (_equippedItems[4] != "NO ACCESSORY")
                accessoryDef = Utils.ItemInfo.GetItemFromString(_equippedItems[4]).Damage;

            return headgearDef + footgearDef + chestgearDef + accessoryDef;
        }

        private void EquipItem(ref Texture2D equipItemSprite, int equipItemIndex, Inventory.ITEMTYPE equipItemType)
        {
            if (equipItemSprite == null)
            {
                _equippedItems[equipItemIndex] = _playerInventory.GetSelectedItem().Name.ToUpper();
                equipItemSprite = _playerInventory.GetSelectedItem().Sprite;

                if (equipItemType == Inventory.ITEMTYPE.WEAPON)
                {
                    _equippedWeapon = _playerInventory.GetSelectedItem();                    
                    _fightOptions = Utils.CombatHandler.SetCombatOptions(_playerInventory.GetSelectedItem().Name);
                }
            }
            else
            {
                equipItemSprite = null;
                _equippedItems[equipItemIndex] = "NO " + equipItemType.ToString().ToUpper();

                if (equipItemType == Inventory.ITEMTYPE.WEAPON)
                {
                    _equippedWeapon = null;                    
                    _fightOptions = Utils.CombatHandler.SetCombatOptions("none");
                }
            }
        }

        private void UpdateInput(Utils.WorldMap wMap, ContentManager contentManager)
        {
            if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Left)
                && CanMoveToTile(new Vector2(Position.X - Utils.WorldMap.UNITSIZE, Position.Y), wMap))
            {
                Position = new Vector2(Position.X - Utils.WorldMap.UNITSIZE, Position.Y);
                _steps++;

                UpdateStats();
                CheckTileResource(Position, wMap, contentManager);
            }
            else if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Right)
                && CanMoveToTile(new Vector2(Position.X + Utils.WorldMap.UNITSIZE, Position.Y), wMap))
            {
                Position = new Vector2(Position.X + Utils.WorldMap.UNITSIZE, Position.Y);
                _steps++;

                UpdateStats();
                CheckTileResource(Position, wMap, contentManager);
            }
            else if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Up)
                && CanMoveToTile(new Vector2(Position.X, Position.Y - Utils.WorldMap.UNITSIZE), wMap))
            {
                Position = new Vector2(Position.X, Position.Y - Utils.WorldMap.UNITSIZE);
                _steps++;

                UpdateStats();
                CheckTileResource(Position, wMap, contentManager);
            }
            else if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Down)
                && CanMoveToTile(new Vector2(Position.X, Position.Y + Utils.WorldMap.UNITSIZE), wMap))
            {
                Position = new Vector2(Position.X, Position.Y + Utils.WorldMap.UNITSIZE);
                _steps++;

                UpdateStats();
                CheckTileResource(Position, wMap, contentManager);
            }
        }

        public string GetCauseOfDeath()
        {
            string cause = "";

            if (_stats.Hunger <= 0)
            {
                cause += "HUNGER";
            }
            if (_stats.Hydration <= 0)
            {
                if (cause.Length > 0)
                    cause += " and DEHYDRATION";
                else
                    cause += "DEHYDRATION";
            }
            if (_stats.Illness != AdvancedStats.ILLNESSES.NONE)
            {
                cause = _stats.Illness.ToString();
            }

            return cause;
        }

        private void UpdateStats()
        {
            if (_stats.Illness == AdvancedStats.ILLNESSES.CHOLERA)
                _stats.Hydration -= 1;
            if (_stats.Illness == AdvancedStats.ILLNESSES.MOUNTAINFEVER)
            {
                if (_stats.HP > 1)
                    _stats.HP -= 1;

                if (_stats.Speed < 5f)
                    _stats.Speed += 0.1f;
            }
            if (_stats.Illness == AdvancedStats.ILLNESSES.SCURVY)
            {
                _stats.HP -= 1;
            }
            if (_stats.Illness == AdvancedStats.ILLNESSES.PNEUMONIA)
            {
                _stats.Speed += 0.05f;

                if (_stats.Speed > 6f)
                    _stats.HP -= 5;
            }

            if (_steps % _stats.Stamina == 0 && !_graphicsManager.IsCampfireAffectedTile((int)Position.X, (int)Position.Y))
            {
                _stats.Hydration -= 1;
                _steps = 0;
            }
            if (_steps % (_stats.Stamina / 2) == 0 && !_graphicsManager.IsCampfireAffectedTile((int)Position.X, (int)Position.Y))
            {
                _stats.Hunger -= 1;
            }

            if (_stats.Hunger <= 0)
                _stats.HP -= 1;
            if (_stats.Hydration <= 0)
                _stats.HP -= 2;
        }

        private void UpdateMovement(Utils.WorldMap wMap)
        {
            if (_canMove)
            {
                if (wMap.GetTileType(new Vector2(Position.X, Position.Y)) == Utils.WorldMap.MAPTILETYPE.TREE)
                {
                    _inputManager.MoveSpeed = 0.5 * _stats.Speed;
                    _fieldOfView = 7;
                }
                else if (wMap.GetTileType(new Vector2(Position.X, Position.Y)) == Utils.WorldMap.MAPTILETYPE.EVERGREEN)
                {
                    _inputManager.MoveSpeed = 1.5 * _stats.Speed;
                    _fieldOfView = 5;
                }
                else
                {
                    _inputManager.MoveSpeed = 0.15 * _stats.Speed;
                    _fieldOfView = 10;
                }
            }
            else
            {
                _inputManager.MoveSpeed = 0.15;
            }
        }

        private bool CanMoveToTile(Vector2 movePos, Utils.WorldMap wMap, string dungeonName = "")
        {
            return CheckOverworldTiles(movePos, wMap);
        }

        private bool CheckOverworldTiles(Vector2 pos, Utils.WorldMap map)
        {
            if (map.GetTileType(new Vector2(pos.X, pos.Y)) != Utils.WorldMap.MAPTILETYPE.WATER &&
                map.GetTileType(new Vector2(pos.X, pos.Y)) != Utils.WorldMap.MAPTILETYPE.MOUNTAIN &&
                map.GetTileType(new Vector2(pos.X, pos.Y)) != Utils.WorldMap.MAPTILETYPE.OUTOFBOUNDS &&
                map.GetTileType(new Vector2(pos.X, pos.Y)) != Utils.WorldMap.MAPTILETYPE.CAMPFIRE)
            {
                return true;
            }
            else if (map.GetTileType(new Vector2(pos.X, pos.Y)) == Utils.WorldMap.MAPTILETYPE.OUTOFBOUNDS)
            {
                if (pos.X < 0)
                    Position = new Vector2(map.MapWidth * Utils.WorldMap.UNITSIZE, Position.Y);
                else if (pos.Y < 0)
                    Position = new Vector2(Position.X, map.MapWidth * Utils.WorldMap.UNITSIZE);
                else if (pos.Y >= (map.MapWidth * Utils.WorldMap.UNITSIZE))
                    Position = new Vector2(Position.X, 0 - Utils.WorldMap.UNITSIZE);
                else if (pos.X >= (map.MapWidth * Utils.WorldMap.UNITSIZE))
                    Position = new Vector2(0 - Utils.WorldMap.UNITSIZE, Position.Y);
                return true;
            }

            return false;
        }

        private void SetIngestedIllness(Inventory.ITEMTYPE food)
        {
            switch (food)
            {
                case Inventory.ITEMTYPE.THYME:
                    break;
                case Inventory.ITEMTYPE.OREGANO:
                    if (_stats.Illness == AdvancedStats.ILLNESSES.MOUNTAINFEVER)
                    {
                        _stats.Speed = 1;
                        _stats.Illness = AdvancedStats.ILLNESSES.NONE;
                    }
                    break;
                case Inventory.ITEMTYPE.GARLIC:
                    if (_stats.Illness == AdvancedStats.ILLNESSES.CHOLERA)
                        _stats.Illness = AdvancedStats.ILLNESSES.NONE;
                    break;
            }
        }
    }
}
