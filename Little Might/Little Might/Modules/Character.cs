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

        private string[] _interactionOptions;
        private int _interactIndex = 0;

        private int _craftIndex = 0;
        private Modules.Inventory.ITEMTYPE[] _craftingList = {
            Modules.Inventory.ITEMTYPE.NONE,
            Modules.Inventory.ITEMTYPE.NONE,
            Modules.Inventory.ITEMTYPE.NONE,
            Modules.Inventory.ITEMTYPE.NONE
        };

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

        public String[] InteractionOptions
        {
            get { return _interactionOptions; }
        }

        public Character (string spriteName, string interactSpriteName, Vector2 startingPosition, ContentManager contentManager, Utils.GraphicsManager gManager, Game1 gameClass)
        {
            _inputManager = new Utils.InputManager();
            _stats = new AdvancedStats(50, 10, 0, 1, 10, 10, 10, 10, 10, 0f);
            _playerInventory = new Inventory(contentManager);
            _interactionOptions = new string[] { "Attempt Communication -", "Fight", "Runaway" };

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
                UpdateMovementSpeed(map);
                UpdateInventoryInteraction(content, map);

                if (_canMove)
                {
                    UpdateInput(map, content);
                }
            }
            else
            {
                if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    if (_interactIndex + 1 < _interactionOptions.Length)
                    {
                        _interactionOptions[_interactIndex] = _interactionOptions[_interactIndex].Remove(_interactionOptions[_interactIndex].Length - 2, 2);
                        _interactIndex++;
                        _interactionOptions[_interactIndex] += " -";
                    }
                }
                else if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    if (_interactIndex - 1 >= 0)
                    {
                        _interactionOptions[_interactIndex] = _interactionOptions[_interactIndex].Remove(_interactionOptions[_interactIndex].Length - 2, 2);
                        _interactIndex--;
                        _interactionOptions[_interactIndex] += " -";
                    }
                }

                if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Enter) &&
                    _interactionOptions[_interactIndex] == "Runaway -")
                {
                    _interactionOptions[_interactIndex] = _interactionOptions[_interactIndex].Remove(_interactionOptions[_interactIndex].Length - 2, 2);
                    _interactIndex = 0;
                    _interactionOptions[_interactIndex] += " -";
                    _game.LeaveInteraction();
                }
            }
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

            if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                if (_playerInventory.GetSelectedItem() == null)
                    return;

                Inventory.ITEMTYPE _itemType = _playerInventory.GetSelectedItem().Type;
                int _hungerValue = Utils.ItemInfo.GetItemHungerValue(_itemType);
                _playerInventory.GetSelectedItem().Toggled = !_playerInventory.GetSelectedItem().Toggled;

                if (_hungerValue != 0)
                {
                    _stats.Hunger += _hungerValue;
                    _graphicsManager.ShowSystemMessage("Ate " + _playerInventory.GetSelectedItem().Type.ToString() + "\n+" + _hungerValue.ToString() + " HUNGER");
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

                Inventory.ITEMTYPE _craftedType = Utils.ItemInfo.CheckCraftables(_craftingList);

                if (_craftedType != Inventory.ITEMTYPE.NONE)
                {
                    _playerInventory.RemoveToggledItems();
                    _playerInventory.AddItem(_craftedType, content);

                    _craftIndex = 0;
                    Array.Clear(_craftingList, 0, _craftingList.Length);

                    _graphicsManager.ShowSystemMessage("Made " + _craftedType + "!");
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

        private void UpdateMovementSpeed(Utils.WorldMap wMap)
        {
            if (_canMove)
            {
                if (wMap.GetTileType(new Vector2(Position.X, Position.Y)) == Utils.WorldMap.MAPTILETYPE.TREE)
                {
                    _inputManager.MoveSpeed = 0.5 * _stats.Speed;
                }
                else if (wMap.GetTileType(new Vector2(Position.X, Position.Y)) == Utils.WorldMap.MAPTILETYPE.EVERGREEN)
                {
                    _inputManager.MoveSpeed = 1.5 * _stats.Speed;
                }
                else
                {
                    _inputManager.MoveSpeed = 0.15 * _stats.Speed;
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
    }
}
