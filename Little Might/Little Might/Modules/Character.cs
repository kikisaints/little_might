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

        public Character (string spriteName, string interactSpriteName, Vector2 startingPosition, ContentManager contentManager, Utils.GraphicsManager gManager, Game1 gameClass)
        {
            _inputManager = new Utils.InputManager();
            _stats = new AdvancedStats(50, 10, 0, 1, 10, 10, 10, 10, 10, 2f, 0f);
            _playerInventory = new Inventory(contentManager);
            _interactionOptions = new string[] { "Communicate -", "Fight", "Run Away" };
            _fightOptions = Utils.CombatHandler.SetCombatOptions("none");
            _equippedItems = new string[] { "NO WEAPON", "HEADGEAR", "FOOTGEAR", "CHESTGEAR", "ACCESSORY" };

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
                if (_playerInventory.AddItem(Inventory.ITEMTYPE.FRUIT, content))
                    _graphicsManager.ShowSystemMessage("Picked up " + Inventory.ITEMTYPE.FRUIT.ToString());
            }
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.BUSH)
            {
                Inventory.ITEMTYPE randomBushItem = wMap.GetBushItem();

                if (randomBushItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomBushItem, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomBushItem.ToString());
                }
            }
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.HERBS)
            {
                Inventory.ITEMTYPE randomHerbItem = wMap.GetHerbItem();

                if (randomHerbItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomHerbItem, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomHerbItem.ToString());
                }
            }
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.ROCK)
            {
                Inventory.ITEMTYPE randomRockItem = wMap.GetStoneItem();

                if (randomRockItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomRockItem, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomRockItem.ToString());
                }
            }
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.TREE)
            {
                Inventory.ITEMTYPE randomTreeItem = wMap.GetTreeItem();

                if (randomTreeItem != Inventory.ITEMTYPE.NONE)
                {
                    if (_playerInventory.AddItem(randomTreeItem, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomTreeItem.ToString());
                }
            }
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.ITEMDROP)
            {
                WorldObject obj = _graphicsManager.GetWorldObject((int)movePos.X, (int)movePos.Y);

                if (obj.ObjectColor == Utils.GameColors.MonsterSlimeColor)
                {
                    if (_playerInventory.AddItem(Inventory.ITEMTYPE.GOOP, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + Inventory.ITEMTYPE.GOOP.ToString());
                }
                if (obj.ObjectColor == Utils.GameColors.MonsterRabbitColor)
                {
                    if (_playerInventory.AddItem(Inventory.ITEMTYPE.HARELEG, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + Inventory.ITEMTYPE.HARELEG.ToString());
                }
                if (obj.ObjectColor == Utils.GameColors.MonsterDeerColor)
                {
                    if (_playerInventory.AddItem(Inventory.ITEMTYPE.VEAL, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + Inventory.ITEMTYPE.VEAL.ToString());
                }

                wMap.ChangeTile(new Vector2(Position.X, Position.Y), Utils.WorldMap.MAPTILETYPE.GRASS);
                _graphicsManager.ChangeWorldObjectVisual(content.Load<Texture2D>("tile_grass"),
                    Utils.GameColors.GrassMapColor,
                    (int)Position.X,
                    (int)Position.Y,
                    Modules.Inventory.ITEMTYPE.NONE);
            }
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.CHEST)
            {
                Inventory.ITEMTYPE randomChestItem = wMap.GetChestItem();

                if (randomChestItem == Inventory.ITEMTYPE.WEAPON)
                {
                    int chance = Utils.MathHandler.GetRandomNumber(0, 1);
                    if (chance == 0)
                    {
                        if (_playerInventory.AddItem(Utils.ItemInfo.GetItemByName("steelsword", content)))
                            _graphicsManager.ShowSystemMessage("Picked up STEELSWORD");                        
                    }
                    else if (chance == 1)
                    {
                        if (_playerInventory.AddItem(Utils.ItemInfo.GetItemByName("stonesword", content)))
                            _graphicsManager.ShowSystemMessage("Picked up STONESWORD");
                    }
                }
                else
                {
                    if (_playerInventory.AddItem(randomChestItem, content))
                        _graphicsManager.ShowSystemMessage("Picked up " + randomChestItem.ToString());
                }

                wMap.ChangeTile(new Vector2(Position.X, Position.Y), Utils.WorldMap.MAPTILETYPE.GRASS);
                _graphicsManager.ChangeWorldObjectVisual(content.Load<Texture2D>("tile_grass"),
                    Utils.GameColors.GrassMapColor,
                    (int)Position.X,
                    (int)Position.Y,
                    Modules.Inventory.ITEMTYPE.NONE);
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
                        _graphicsManager.ShowSystemMessage("Trashed " + _playerInventory.GetSelectedItem().Type.ToString().ToUpper() + "!");
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

                    Inventory.ITEMTYPE _itemType = _playerInventory.GetSelectedItem().Type;

                    if (_itemType == Inventory.ITEMTYPE.WEAPON)
                    {
                        if (_weaponSprite == null)
                        {
                            _equippedWeapon = _playerInventory.GetSelectedItem();
                            _weaponSprite = _playerInventory.GetSelectedItem().Sprite;
                            _equippedItems[0] = _playerInventory.GetSelectedItem().Name.ToUpper();

                            _fightOptions = Utils.CombatHandler.SetCombatOptions(_playerInventory.GetSelectedItem().Name);
                        }
                        else
                        {
                            _equippedWeapon = null;
                            _weaponSprite = null;
                            _equippedItems[0] = "NO WEAPON";
                            _fightOptions = Utils.CombatHandler.SetCombatOptions("none");
                        }

                        return;
                    }

                    int _hungerValue = Utils.ItemInfo.GetItemHungerValue(_itemType);
                    _playerInventory.GetSelectedItem().Toggled = !_playerInventory.GetSelectedItem().Toggled;

                    if (_hungerValue != 0)
                    {
                        SetIngestedIllness(_playerInventory.GetSelectedItem().Type);

                        _stats.Hunger += _hungerValue;
                        _graphicsManager.ShowSystemMessage("+" + _hungerValue.ToString() + " HUNGER");
                        _playerInventory.RemoveSelectedItem();

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

                    InventoryItem _craftedItem = Utils.ItemInfo.CheckCraftables(_craftingList, _playerInventory, content);

                    if (_craftedItem != null)
                    {
                        _playerInventory.AddItem(_craftedItem);
                        _playerInventory.RemoveToggledItems();                        

                        _craftIndex = 0;
                        Array.Clear(_craftingList, 0, _craftingList.Length);

                        _graphicsManager.ShowSystemMessage("Made " + _craftedItem.Name.ToUpper() + "!");
                    }
                    else if (_playerInventory.GetSelectedItem().IsPlaceable)
                    {
                        if (_itemType == Inventory.ITEMTYPE.CAMPFIRE)
                        {
                            map.ChangeTile(new Vector2(Position.X, Position.Y + Utils.WorldMap.UNITSIZE), Utils.WorldMap.MAPTILETYPE.CAMPFIRE);
                            _graphicsManager.ChangeWorldObjectVisual(content.Load<Texture2D>("campfire"),
                                Utils.GameColors.CampfireMapColor,
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

            if (_steps % _stats.Stamina == 0)
            {
                _stats.Hydration -= 1;
                _steps = 0;
            }
            if (_steps % (_stats.Stamina / 2) == 0)
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

        private bool CanMoveToTile(Vector2 movePos, Utils.WorldMap wMap)
        {
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) != Utils.WorldMap.MAPTILETYPE.WATER &&
                wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) != Utils.WorldMap.MAPTILETYPE.MOUNTAIN &&
                wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) != Utils.WorldMap.MAPTILETYPE.OUTOFBOUNDS &&
                wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) != Utils.WorldMap.MAPTILETYPE.CAMPFIRE)
                return true;
            return false;
        }

        private void SetIngestedIllness(Inventory.ITEMTYPE food)
        {
            switch (food.ToString().ToLower())
            {
                case "thyme":
                    break;
                case "oregano":
                    if (_stats.Illness == AdvancedStats.ILLNESSES.MOUNTAINFEVER)
                    {
                        _stats.Speed = 1;
                        _stats.Illness = AdvancedStats.ILLNESSES.NONE;
                    }
                    break;
                case "garlic":
                    if (_stats.Illness == AdvancedStats.ILLNESSES.CHOLERA)
                        _stats.Illness = AdvancedStats.ILLNESSES.NONE;
                    break;
            }
        }
    }
}
