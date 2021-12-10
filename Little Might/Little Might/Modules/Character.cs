﻿using System;
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
            _playerInventory = new Inventory();

            Sprite = contentManager.Load<Texture2D>(spriteFileName);
            Position = startingPosition;
            ObjectColor = Color.White;
            _graphicsManager = gManager;
        }

        public void UpdateCharacter(GameTime time, Utils.WorldMap map, ContentManager content)
        {
            _inputManager.UpdateInput(time);
            UpdateInput(map, content);
            UpdateMovementSpeed(map);            
        }

        private void CheckTileResource(Vector2 movePos, Utils.WorldMap wMap, ContentManager content)
        {
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.FRUIT)
            {
                _playerInventory.AddItem(Inventory.ITEMTYPE.FRUIT, content);
                _graphicsManager.ShowSystemMessage("Picked up " + Inventory.ITEMTYPE.FRUIT.ToString());
            }

            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.BUSH)
            {
                Inventory.ITEMTYPE randomBushItem = wMap.GetBushItem();

                if (randomBushItem != Inventory.ITEMTYPE.NONE)
                {
                    _playerInventory.AddItem(randomBushItem, content);
                    _graphicsManager.ShowSystemMessage("Picked up " + randomBushItem.ToString());
                }
            }
            if (wMap.GetTileType(new Vector2(movePos.X, movePos.Y)) == Utils.WorldMap.MAPTILETYPE.ROCK)
            {
                Inventory.ITEMTYPE randomRockItem = wMap.GetStoneItem();

                if (randomRockItem != Inventory.ITEMTYPE.NONE)
                {
                    _playerInventory.AddItem(randomRockItem, content);
                    _graphicsManager.ShowSystemMessage("Picked up " + randomRockItem.ToString());
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

            if (_stats.Hunger < 50 && _stats.Hunger > 25)
                _graphicsManager.ShowSystemMessage("You are HUNGRY");
            else if (_stats.Hunger <= 25)
                _graphicsManager.ShowSystemMessage("You are STARVING");

            if (_stats.Hydration < 50 && _stats.Hydration > 25)
                _graphicsManager.ShowSystemMessage("You are THIRSTY");
            else if (_stats.Hydration <= 25)
                _graphicsManager.ShowSystemMessage("You are DEHYDRATED");
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
