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

        private int _steps = 0;
        private bool _canMove = true;

        public Inventory Inv
        {
            get { return _playerInventory; }
        }

        public AdvancedStats Stats
        {
            get { return _stats; }
        }

        public Character (string spriteFileName, Vector2 startingPosition, ContentManager contentManager, Utils.GraphicsManager gManager)
        {
            _inputManager = new Utils.InputManager();
            _stats = new AdvancedStats(50, 10, 0, 1, 10, 10, 10, 10, 10, 0f);
            _playerInventory = new Inventory(contentManager);

            Sprite = contentManager.Load<Texture2D>(spriteFileName);
            Position = startingPosition;
            ObjectColor = Color.White;
            _graphicsManager = gManager;
        }

        public void UpdateCharacter(GameTime time, Utils.WorldMap map, ContentManager content)
        {
            _inputManager.UpdateInput(time);
            UpdateInventoryInteraction();

            if (_canMove)
            {
                UpdateInput(map, content);
                UpdateMovementSpeed(map);
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
        }

        private void UpdateInventoryInteraction()
        {
            if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.I))
            {
                _canMove = !_canMove;
                _playerInventory.NavigatingInventory = !_playerInventory.NavigatingInventory;
            }

            if (_playerInventory.NavigatingInventory)
            {
                _playerInventory.UpdateInventory(_inputManager);
            }

            //TEMP - Need to be smarter/more componentized about how info is grabbed for each item type...
            if (_inputManager.ButtonToggled(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                if (_playerInventory.GetSelectedItem() == Inventory.ITEMTYPE.FRUIT)
                {
                    _stats.Hunger += 35;
                    _graphicsManager.ShowSystemMessage("Ate " + Inventory.ITEMTYPE.FRUIT.ToString() + "\n+35 HUNGER");
                    _playerInventory.RemoveSelectedItem(); //temp
                }
                else if (_playerInventory.GetSelectedItem() == Inventory.ITEMTYPE.BERRY)
                {
                    _stats.Hunger += 15;
                    _graphicsManager.ShowSystemMessage("Ate " + Inventory.ITEMTYPE.BERRY.ToString() + "! \n+15 HUNGER");
                    _playerInventory.RemoveSelectedItem(); //temp
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

        private bool CanMoveToTile(Vector2 movePos, Utils.WorldMap wMap)
        {
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) != Utils.WorldMap.MAPTILETYPE.WATER &&
                wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) != Utils.WorldMap.MAPTILETYPE.MOUNTAIN &&
                wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) != Utils.WorldMap.MAPTILETYPE.OUTOFBOUNDS)
                return true;
            return false;
        }
    }
}
