using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//debug
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Little_Might.Utils
{
    class GraphicsManager
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Modules.WorldObject> _worldObjects;
        private List<Modules.WorldObject> _characters;
        private List<Modules.ScreenObject> _screenObjects;
        private SpriteFont _font;
        private Effect _effect;

        private bool _showSystemUI = false;
        private double _showSysMsgTimer = 0f;
        private double _displayMsgTime = 3;
        private string _systemMessage = "";

        //UI Icons -- TEMP
        private Texture2D _hpIcon;
        private Texture2D _waterIcon;
        private Texture2D _foodIcon;
        private Texture2D _diseaseIcon;
        private Texture2D _systemPopup;
        private WorldMap _worldMap;

        public GraphicsDeviceManager Graphics
        {
            get { return _graphics; }
        }        

        public GraphicsManager(Game gameClass)
        {
            _graphics = new GraphicsDeviceManager(gameClass);
            _worldObjects = new List<Modules.WorldObject>();
            _characters = new List<Modules.WorldObject>();
            _screenObjects = new List<Modules.ScreenObject>();
        }

        public void ClearGraphics()
        {
            _worldObjects.Clear();
            _characters.Clear();
        }

        public void Load(GraphicsDevice graphicsDevice, ContentManager contentManager, GameWindow window)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _font = contentManager.Load<SpriteFont>("fonts/pixelfont");

            //UI ICONS
            _hpIcon = contentManager.Load<Texture2D>("icon_heart");
            _foodIcon = contentManager.Load<Texture2D>("icon_food");
            _waterIcon = contentManager.Load<Texture2D>("icon_water");
            _diseaseIcon = contentManager.Load<Texture2D>("icon_disease");
            _systemPopup = contentManager.Load<Texture2D>("system_notice_ui");

            //Game UI Objects
            AddScreenObject(new Modules.ScreenObject(contentManager.Load<Texture2D>("inventory_ui"), new Vector2(-290, 225), 4f));

            SetupCRTEffect(contentManager);
        }

        public void VisualizeMap(WorldMap map, ContentManager contentManager)
        {
            _worldMap = map;

            for (int i = 0; i < map.ColorMap.Length; i++)
            {
                int mapX = (i % _worldMap.MapWidth) * Utils.WorldMap.UNITSIZE;
                int mapY = (i / _worldMap.MapWidth) * Utils.WorldMap.UNITSIZE;

                if (map.ColorMap[i] == GameColors.EvergreenForestMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("tree_evergreen"), 
                        new Vector2(mapX, mapY), 
                        GameColors.EvergreenForestMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.EVERGREEN;
                }
                if (map.ColorMap[i] == GameColors.TreeForestMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("tree_regular"),
                        new Vector2(mapX, mapY),
                        GameColors.TreeForestMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.TREE;
                }
                if (map.ColorMap[i] == GameColors.TreeFruitMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("tree_fruit"),
                        new Vector2(mapX, mapY),
                        GameColors.TreeFruitMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.FRUIT;
                }
                if (map.ColorMap[i] == GameColors.WaterMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("tile_water"),
                        new Vector2(mapX, mapY),
                        GameColors.WaterMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.WATER;
                }
                if (map.ColorMap[i] == GameColors.GrassMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("tile_grass"),
                        new Vector2(mapX, mapY),
                        GameColors.GrassMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.GRASS;
                }
                if (map.ColorMap[i] == GameColors.MountainMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("tile_mountain"),
                        new Vector2(mapX, mapY),
                        GameColors.MountainMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.MOUNTAIN;
                }
                if (map.ColorMap[i] == GameColors.BushMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("bush"),
                        new Vector2(mapX, mapY),
                        GameColors.BushMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.BUSH;
                }
                if (map.ColorMap[i] == GameColors.RockMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("rock"),
                        new Vector2(mapX, mapY),
                        GameColors.RockMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.ROCK;
                }
                if (map.ColorMap[i] == Color.Red)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("chest_little"),
                        new Vector2(mapX, mapY),
                        Color.Gold));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.CHEST;
                }
            }
        }

        private void SetupCRTEffect(ContentManager contentManager)
        {
            //CRT FX
            _effect = contentManager.Load<Effect>("shaders/crt_lottes_mg");
            _effect.Parameters["hardScan"]?.SetValue(-8.0f);
            _effect.Parameters["hardPix"]?.SetValue(-3.0f);
            _effect.Parameters["warpX"]?.SetValue(0.031f);
            _effect.Parameters["warpY"]?.SetValue(0.041f);
            _effect.Parameters["maskDark"]?.SetValue(0.5f);
            _effect.Parameters["maskLight"]?.SetValue(1.5f);
            _effect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
            _effect.Parameters["shadowMask"]?.SetValue(3.0f);
            _effect.Parameters["brightboost"]?.SetValue(1.0f);
            _effect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
            _effect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
            _effect.Parameters["bloomAmount"]?.SetValue(0.15f);
            _effect.Parameters["shape"]?.SetValue(2.0f);

            _effect.Parameters["textureSize"].SetValue(new Vector2(1920, 1080));
            _effect.Parameters["videoSize"].SetValue(new Vector2(1920, 1080));
            _effect.Parameters["outputSize"].SetValue(new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        }

        public void AddWorldObject(Modules.WorldObject worldObject) { _worldObjects.Add(worldObject); }

        public void ChangeWorldObjectVisual(Texture2D newSprite, Color newColor, int x, int y, Modules.Inventory.ITEMTYPE type)
        {
            int indexX = x / Utils.WorldMap.UNITSIZE;
            int indexY = y / Utils.WorldMap.UNITSIZE;
            int index = Utils.MathHandler.Get1DIndex(indexY, indexX, _worldMap.MapWidth);

            _worldObjects[index].Sprite = newSprite;
            _worldObjects[index].ObjectColor = newColor;

            if (type == Modules.Inventory.ITEMTYPE.CAMPFIRE)
            {
                int[] fireAffectedArea = GetAffectShape(4, new Vector2(x, y), _worldMap.MapWidth);

                foreach (int i in fireAffectedArea)
                {
                    _worldObjects[i].ObjectColor = Utils.MathHandler.BlendColor(Utils.GameColors.CampfireMapColor, _worldObjects[i].ObjectColor, 0.5f);
                }
            }
        }

        public void AddScreenObject(Modules.ScreenObject screenObject) { _screenObjects.Add(screenObject); }

        public void AddCharacterObject(Modules.WorldObject worldObject) { _characters.Add(worldObject); }
        

        private int[] GetAffectShape(int drawRadius, Vector2 centerPoint, int mapWidth)
        {
            float xCenter = centerPoint.X / Utils.WorldMap.UNITSIZE;
            float yCenter = centerPoint.Y / Utils.WorldMap.UNITSIZE;

            int startFOWIndex = MathHandler.Get1DIndex((int)yCenter, (int)xCenter, mapWidth);

            int drawIndiciesCount = 0;
            int[] drawIndicies = new int[(drawRadius * drawRadius) * 4];

            for (int x = 0; x < drawRadius; x++)
            {
                for (int y = 0; y < drawRadius; y++)
                {
                    int viewIndex = MathHandler.Get1DIndex(y, x, mapWidth);
                    viewIndex += startFOWIndex;

                    int viewIndexA = MathHandler.Get1DIndex((y - (y * 2)), x, mapWidth);
                    int viewIndexB = MathHandler.Get1DIndex(y, (x - (x * 2)), mapWidth);
                    int viewIndexC = MathHandler.Get1DIndex((y - (y * 2)), (x - (x * 2)), mapWidth);

                    if (y != 0)                    
                        viewIndexA += startFOWIndex;
                    if (x != 0)
                        viewIndexB += startFOWIndex;
                    if (y != 0 && x != 0)
                        viewIndexC += startFOWIndex;

                    if (Utils.MathHandler.IsPointInCircle(viewIndex % mapWidth, viewIndex / mapWidth, startFOWIndex % mapWidth, startFOWIndex / mapWidth, drawRadius))
                    {
                        if (viewIndex >= 0 && viewIndex < _worldObjects.Count)
                        {
                            drawIndicies[drawIndiciesCount] = viewIndex;
                            drawIndiciesCount++;                            
                        }                       
                    }

                    if (Utils.MathHandler.IsPointInCircle(viewIndexA % mapWidth, viewIndexA / mapWidth, startFOWIndex % mapWidth, startFOWIndex / mapWidth, drawRadius))
                    {
                        if (y != 0 && viewIndexA >= 0 && viewIndexA < _worldObjects.Count)
                        {
                            drawIndicies[drawIndiciesCount] = viewIndexA;
                            drawIndiciesCount++;
                        }
                    }
                    if (Utils.MathHandler.IsPointInCircle(viewIndexB % mapWidth, viewIndexB / mapWidth, startFOWIndex % mapWidth, startFOWIndex / mapWidth, drawRadius))
                    {
                        if (x != 0 && viewIndexB >= 0 && viewIndexB < _worldObjects.Count)
                        {
                            drawIndicies[drawIndiciesCount] = viewIndexB;
                            drawIndiciesCount++;
                        }
                    }

                    if (Utils.MathHandler.IsPointInCircle(viewIndexC % mapWidth, viewIndexC / mapWidth, startFOWIndex % mapWidth, startFOWIndex / mapWidth, drawRadius))
                    {
                        if (y != 0 && x != 0 && viewIndexC >= 0 && viewIndexC < _worldObjects.Count)
                        {
                            drawIndicies[drawIndiciesCount] = viewIndexC;
                            drawIndiciesCount++;
                        }
                    }
                }
            }

            return drawIndicies;
        }

        private void DrawGame(int radius, Vector2 charPos, int width, Matrix cameraTransform, bool world = true)
        {
            _spriteBatch.GraphicsDevice.Clear(Utils.GameColors.MainBackgroundColor);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, cameraTransform);
            //_effect.CurrentTechnique.Passes[0].Apply();

            if (world)
            {
                int[] drawArray = GetAffectShape(radius, charPos, width);

                foreach (int i in drawArray)
                {
                    _spriteBatch.Draw(_worldObjects[i].Sprite, _worldObjects[i].Position, null, _worldObjects[i].ObjectColor);
                }

                foreach (Modules.WorldObject cobj in _characters)
                {
                    _spriteBatch.Draw(cobj.Sprite, cobj.Position, null, cobj.ObjectColor);
                }
            }

            _spriteBatch.End();
        }

        public void DrawUIString(string[] text, Vector2 offset, float[] scale)
        {
            _spriteBatch.GraphicsDevice.Clear(Utils.GameColors.MainBackgroundColor);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            for (int s = 0; s < text.Length; s++)
            {
                Vector2 FontOrigin = _font.MeasureString(text[s]);
                _spriteBatch.DrawString(_font, text[s], new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + offset.X, (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + (offset.Y * s)), Color.White, 0, FontOrigin, scale[s], SpriteEffects.None, 0.5f);
            }
            _spriteBatch.End();
        }

        private void DisplayOverworldUI(Modules.Character character, Vector2 playerOrigin)
        {
            Vector2 offset = new Vector2(75, 0);

            //Show character's coordinates
            float xCenter = playerOrigin.X / Utils.WorldMap.UNITSIZE;
            float yCenter = playerOrigin.Y / Utils.WorldMap.UNITSIZE;

            //DEBUG COORDINATES
            string output = xCenter.ToString() + ", " + yCenter.ToString();
            Vector2 FontOrigin = _font.MeasureString(output) / 2;
            _spriteBatch.DrawString(_font, output, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, 25), Color.White, 0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

            //STAT TRACKERS
            //HP
            string health = character.Stats.HP.ToString();
            FontOrigin = _font.MeasureString(health);
            _spriteBatch.DrawString(_font, health, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 150, 50) + offset, Color.White, 0, FontOrigin, 2f, SpriteEffects.None, 0.5f);

            _spriteBatch.Draw(_hpIcon, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 135, 18) + offset, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);

            //Hunger
            string hunger = character.Stats.Hunger.ToString();
            FontOrigin = _font.MeasureString(hunger);
            _spriteBatch.DrawString(_font, hunger, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 150, 100) + offset, Color.White, 0, FontOrigin, 2f, SpriteEffects.None, 0.5f);

            _spriteBatch.Draw(_foodIcon, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 135, 68) + offset, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);

            //Water
            string hydration = character.Stats.Hydration.ToString();
            FontOrigin = _font.MeasureString(hydration);
            _spriteBatch.DrawString(_font, hydration, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 150, 150) + offset, Color.White, 0, FontOrigin, 2f, SpriteEffects.None, 0.5f);

            _spriteBatch.Draw(_waterIcon, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 135, 118) + offset, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);

            //Water
            string illness = character.Stats.Illness.ToString();
            FontOrigin = _font.MeasureString(illness);
            _spriteBatch.DrawString(_font, illness, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 150, 200) + offset, Color.White, 0, FontOrigin, 2f, SpriteEffects.None, 0.5f);

            _spriteBatch.Draw(_diseaseIcon, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width - 135, 168) + offset, null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);

            //DRAW UI OBJECTS            
            foreach (Modules.ScreenObject screenObj in _screenObjects)
            {
                _spriteBatch.Draw(screenObj.Sprite,
                    screenObj.Position + new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width, 0),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    screenObj.Scale,
                    SpriteEffects.None,
                    0.5f);
            }

            foreach (Modules.InventoryItem item in character.Inv.Items)
            {
                _spriteBatch.Draw(item.Sprite,
                    item.Position + new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width, 0),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    item.Scale,
                    SpriteEffects.None,
                    0.5f);
            }

            if (character.Inv.NavigatingInventory)
            {
                _spriteBatch.Draw(character.Inv.InventorySelector.Sprite,
                    character.Inv.InventorySelector.Position + new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width, 0),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    character.Inv.InventorySelector.Scale,
                    SpriteEffects.None,
                    0.5f);

                //Display item info here
                Modules.InventoryItem _item = character.Inv.GetSelectedItem();
                if (_item != null)
                {
                    FontOrigin = _font.MeasureString(_item.Name.ToUpper()) / 2;
                    _spriteBatch.DrawString(_font,
                        _item.Name.ToUpper(),
                        new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 800, (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + 125),
                        Color.White,
                        0,
                        FontOrigin,
                        1f,
                        SpriteEffects.None,
                        1f);

                    FontOrigin = _font.MeasureString(_item.Discription) / 2;
                    _spriteBatch.DrawString(_font,
                        _item.Discription + "\n\n" + "Selected: " + _item.Toggled,
                        new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 800, (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + 160),
                        Color.White,
                        0,
                        FontOrigin,
                        0.75f,
                        SpriteEffects.None,
                        1f);
                }
            }

            if (_showSystemUI)
            {
                FontOrigin = _font.MeasureString(_systemMessage) / 2;
                _spriteBatch.DrawString(_font, _systemMessage, new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 35, (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + 455), Color.Black, 0, FontOrigin, 1f, SpriteEffects.None, 1f);
                _spriteBatch.Draw(_systemPopup, new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) - 150, (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + 400), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.1f);
            }
        }

        private void DrawUI(Vector2 characterPoint, Modules.Character character, bool world = true)
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            if (world)
                DisplayOverworldUI(character, characterPoint);

            _spriteBatch.End();
        }

        public void ShowSystemMessage(string message)
        {
            if (!_showSystemUI)
                _showSystemUI = true;
            else
                _displayMsgTime = 0f;

            _systemMessage = message;
            _showSysMsgTimer = 0;
        }

        private void UpdateSystemDialogs(GameTime time)
        {
            if (_showSystemUI)
            {
                _showSysMsgTimer += time.ElapsedGameTime.TotalSeconds;

                if (_showSysMsgTimer > _displayMsgTime)
                {
                    _showSysMsgTimer = 0;
                    _showSystemUI = false;
                }
            }
            else if (_displayMsgTime < 1) //"Flash" reset the dialog to show that a new action has occurred
            {
                _showSystemUI = false;
                _showSysMsgTimer += time.ElapsedGameTime.Milliseconds;

                if (_showSysMsgTimer >= 150)
                {
                    _showSysMsgTimer = 0;
                    _showSystemUI = true;
                    _displayMsgTime = 3;
                }
            }
        }

        public void DrawUpdate(Matrix cameraMatrix, int viewRadius, Vector2 center, int mapWidth, Modules.Character player, GameTime time, bool overworld = true)
        {
            DrawGame(viewRadius, center, mapWidth, cameraMatrix, overworld);
            DrawUI(center, player, overworld);

            UpdateSystemDialogs(time);
        }
    }
}
