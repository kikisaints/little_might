using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

//debug

namespace Little_Might.Utils
{
    internal class GraphicsManager
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Modules.WorldObject> _worldObjects;
        private List<Modules.WorldObject> _worldObjects_l1;
        private List<Modules.WorldObject> _characters;
        private List<Modules.ScreenObject> _screenObjects;
        private SpriteFont _font;
        private Effect _effect;
        private ContentManager _content;

        private bool _showSystemUI = false;
        private bool _showInteractionOptions = true;
        private double _showSysMsgTimer = 0f;
        private double _displayMsgTime = 2;
        private string _systemMessage = "";

        private int _activeDrawLayer = 0;

        //UI Icons -- TEMP
        private Texture2D _hpIcon;

        private Texture2D _waterIcon;
        private Texture2D _foodIcon;
        private Texture2D _diseaseIcon;
        private Texture2D _systemPopup;
        private WorldMap _worldMap;

        private Random rand = new Random(2589);

        private SoundEffect systemMessage;

        public void SetDrawLayer(int newLayer)
        { _activeDrawLayer = newLayer; }

        public GraphicsDeviceManager Graphics
        {
            get { return _graphics; }
        }

        public List<Modules.WorldObject> Characters
        {
            get { return _characters; }
        }

        public GraphicsManager(Game gameClass)
        {
            _graphics = new GraphicsDeviceManager(gameClass);
            _worldObjects = new List<Modules.WorldObject>();
            _worldObjects_l1 = new List<Modules.WorldObject>();
            _characters = new List<Modules.WorldObject>();
            _screenObjects = new List<Modules.ScreenObject>();
        }

        public void ClearGraphics()
        {
            _worldObjects.Clear();
            _characters.Clear();
        }

        public void DisplayInteractionOptions(bool value)
        {
            _showInteractionOptions = value;
        }

        public void Load(GraphicsDevice graphicsDevice, ContentManager contentManager, GameWindow window)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _font = contentManager.Load<SpriteFont>("fonts/pixelfont");

            //UI ICONS
            _hpIcon = contentManager.Load<Texture2D>("images/icon_heart");
            _foodIcon = contentManager.Load<Texture2D>("images/icon_food");
            _waterIcon = contentManager.Load<Texture2D>("images/icon_water");
            _diseaseIcon = contentManager.Load<Texture2D>("images/icon_disease");
            _systemPopup = contentManager.Load<Texture2D>("images/system_notice_ui");

            _content = contentManager;

            //Game UI Objects
            AddScreenObject(new Modules.ScreenObject(contentManager.Load<Texture2D>("images/inventory_ui"), new Vector2(-290, 225), 4f));
            SetupCRTEffect(contentManager);

            //Sounds
            systemMessage = contentManager.Load<SoundEffect>("sound/pickupCoin");
        }

        public void VisualizeWorld(WorldMap map, ContentManager contentManager)
        {
            _worldMap = map;

            for (int i = 0; i < map.ColorMap.Length; i++)
            {
                int mapX = (i % _worldMap.MapWidth) * Utils.WorldMap.UNITSIZE;
                int mapY = (i / _worldMap.MapWidth) * Utils.WorldMap.UNITSIZE;

                if (map.ColorMap[i] == GameColors.EvergreenForestMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tree_evergreen"),
                        new Vector2(mapX, mapY),
                        GameColors.EvergreenForestMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.EVERGREEN;
                }
                if (map.ColorMap[i] == GameColors.TreeForestMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tree_regular"),
                        new Vector2(mapX, mapY),
                        GameColors.TreeForestMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.TREE;
                }
                if (map.ColorMap[i] == GameColors.TreeFruitMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tree_fruit"),
                        new Vector2(mapX, mapY),
                        GameColors.TreeFruitMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.FRUIT;
                }
                if (map.ColorMap[i] == GameColors.WaterMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tile_water"),
                        new Vector2(mapX, mapY),
                        GameColors.WaterMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.WATER;
                }
                if (map.ColorMap[i] == GameColors.GrassMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tile_grass"),
                        new Vector2(mapX, mapY),
                        GameColors.GrassMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.GRASS;
                }
                if (map.ColorMap[i] == GameColors.MountainMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tile_mountain"),
                        new Vector2(mapX, mapY),
                        GameColors.MountainMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.MOUNTAIN;
                }
                if (map.ColorMap[i] == GameColors.BushMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/bush"),
                        new Vector2(mapX, mapY),
                        GameColors.BushMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.BUSH;
                }
                if (map.ColorMap[i] == GameColors.RockMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/rock"),
                        new Vector2(mapX, mapY),
                        GameColors.RockMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.ROCK;
                }
                if (map.ColorMap[i] == Color.Red)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/chest_little"),
                        new Vector2(mapX, mapY),
                        GameColors.ChestMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.CHEST;
                }
                if (map.ColorMap[i] == GameColors.HerbMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tile_herbs"),
                        new Vector2(mapX, mapY),
                        GameColors.HerbMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.HERBS;
                }
                if (map.ColorMap[i] == GameColors.PrarieDungeonMapColor)
                {
                    AddWorldObject(new Modules.WorldObject(contentManager.Load<Texture2D>("images/dungeon_entrance_1"),
                        new Vector2(mapX, mapY),
                        GameColors.PrarieDungeonMapColor));

                    map.MapTiles[mapX, mapY] = WorldMap.MAPTILETYPE.PRARIEDUNGEON;
                }
            }

            foreach (WorldMap.DungeonMap dMap in map.DungeonMaps)
            {
                Vector2 startPos = dMap.DungeonDoor;
                Vector2 offsetPos = dMap.DungeonExitDoor;

                //need to offset from the dungoen door position to the 0,0 in the dungoen map array
                //which is dependent on where the door starts and is not necessarily an even distance
                //Should NOT be hardcoded, need to change that...
                startPos.X = (startPos.X - offsetPos.X) * Utils.WorldMap.UNITSIZE;
                startPos.Y = (startPos.Y - offsetPos.Y) * Utils.WorldMap.UNITSIZE;

                for (int j = 0; j < dMap.Map.Length; j++)
                {
                    int dMapX = (j % dMap.MapWidth) * Utils.WorldMap.UNITSIZE;
                    int dMapY = (j / dMap.MapWidth) * Utils.WorldMap.UNITSIZE;

                    if (dMap.Map[j] == 1)
                    {
                        _worldObjects_l1.Add(new Modules.WorldObject(contentManager.Load<Texture2D>("images/tile_stone"),
                        new Vector2(startPos.X + dMapX, startPos.Y + dMapY),
                        GameColors.StoneMapColor, 1));
                    }
                    else if (dMap.Map[j] == 8)
                    {
                        _worldObjects_l1.Add(new Modules.WorldObject(contentManager.Load<Texture2D>("images/dungeon_entrance_1"),
                        new Vector2(startPos.X + dMapX, startPos.Y + dMapY),
                        GameColors.PrarieDungeonMapColor, 1));
                    }
                    else if (dMap.Map[j] == 2)
                    {
                        _worldObjects_l1.Add(new Modules.WorldObject(contentManager.Load<Texture2D>("images/chest_little"),
                        new Vector2(startPos.X + dMapX, startPos.Y + dMapY),
                        GameColors.ChestMapColor, 1));
                    }
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

        public void AddWorldObject(Modules.WorldObject worldObject)
        { _worldObjects.Add(worldObject); }

        public void ChangeWorldObjectVisual(Texture2D newSprite, Color newColor, int x, int y, Modules.Inventory.ITEMTYPE type,
            Utils.WorldMap.MAPTILETYPE mapTile = WorldMap.MAPTILETYPE.OUTOFBOUNDS, string dungeonName = "", Utils.WorldMap.MAPTILETYPE pastMapTile = WorldMap.MAPTILETYPE.OUTOFBOUNDS)
        {
            if (_activeDrawLayer == 0)
            {
                int indexX = x / Utils.WorldMap.UNITSIZE;
                int indexY = y / Utils.WorldMap.UNITSIZE;
                int index = Utils.MathHandler.Get1DIndex(indexY, indexX, _worldMap.MapWidth);

                _worldObjects[index].Sprite = newSprite;
                _worldObjects[index].ObjectColor = newColor;

                if (mapTile != WorldMap.MAPTILETYPE.OUTOFBOUNDS)
                    _worldMap.MapTiles[x, y] = mapTile;

                if (type == Modules.Inventory.ITEMTYPE.CAMPFIRE || type == Modules.Inventory.ITEMTYPE.FURNACE)
                {
                    int[] fireAffectedArea = GetAffectShape(4, new Vector2(x, y), _worldMap.MapWidth);

                    foreach (int i in fireAffectedArea)
                    {
                        _worldObjects[i].ObjectColor = Utils.MathHandler.BlendColor(Utils.GameColors.CampfireMapColor, _worldObjects[i].ObjectColor, 0.5f);
                        _worldObjects[i].FireAffected = true;
                    }
                }
            }
            else if (_activeDrawLayer == 1)
            {
                Vector2 startPoint = new Vector2(x, y);
                WorldMap.DungeonMap dunMap = _worldMap.GetDungeonByName(dungeonName);

                int indexX = x / Utils.WorldMap.UNITSIZE;
                int indexY = y / Utils.WorldMap.UNITSIZE;
                int index = Utils.MathHandler.Get1DIndex(indexY, indexX, dunMap.MapWidth);

                if (pastMapTile == WorldMap.MAPTILETYPE.CHEST)
                {
                    for (int i = 0; i < dunMap.Map.Length; i++)
                    {
                        if (dunMap.Map[i] == 2)
                        {
                            dunMap.MapWorldPoints[i].TileType = mapTile;
                            dunMap.Map[i] = 1;
                            break;
                        }
                    }
                }

                for (int i = 0; i < _worldObjects_l1.Count; i++)
                {
                    if (Utils.MathHandler.WorldObjectIntersects(startPoint, _worldObjects_l1[i].Position))
                    {
                        _worldObjects_l1[i].Sprite = newSprite;
                        _worldObjects_l1[i].ObjectColor = newColor;
                    }
                }

                if (type == Modules.Inventory.ITEMTYPE.CAMPFIRE || type == Modules.Inventory.ITEMTYPE.FURNACE)
                {
                    foreach (Modules.WorldObject wobj in _worldObjects_l1)
                    {
                        if (Utils.MathHandler.IsPointInCircle((int)wobj.Position.X / Utils.WorldMap.UNITSIZE,
                        (int)wobj.Position.Y / Utils.WorldMap.UNITSIZE,
                        (int)startPoint.X / Utils.WorldMap.UNITSIZE,
                        (int)startPoint.Y / Utils.WorldMap.UNITSIZE,
                        4))
                        {
                            wobj.ObjectColor = Utils.MathHandler.BlendColor(Utils.GameColors.CampfireMapColor, wobj.ObjectColor, 0.5f);
                            wobj.FireAffected = true;
                        }
                    }
                }
            }
        }

        public bool IsCampfireAffectedTile(int x, int y)
        {
            if (_activeDrawLayer == 0)
            {
                int indexX = x / Utils.WorldMap.UNITSIZE;
                int indexY = y / Utils.WorldMap.UNITSIZE;
                int index = Utils.MathHandler.Get1DIndex(indexY, indexX, _worldMap.MapWidth);

                if (index >= _worldObjects.Count || index < 0)
                    return false;

                return _worldObjects[index].FireAffected;
            }
            else if (_activeDrawLayer == 1)
            {
                foreach (Modules.WorldObject wobj_l1 in _worldObjects_l1)
                {
                    if (Utils.MathHandler.WorldObjectIntersects(new Vector2(x, y), wobj_l1.Position))
                        return wobj_l1.FireAffected;
                }
            }

            return false;
        }

        public Modules.WorldObject GetWorldObject(int x, int y, int layer = 0)
        {
            int indexX = x / Utils.WorldMap.UNITSIZE;
            int indexY = y / Utils.WorldMap.UNITSIZE;
            int index = Utils.MathHandler.Get1DIndex(indexY, indexX, _worldMap.MapWidth);

            if (layer == 0)
            {
                return _worldObjects[index];
            }
            else if (layer == 1)
            {
                foreach (Modules.WorldObject wobj in _worldObjects_l1)
                {
                    if ((int)wobj.Position.X == x && (int)wobj.Position.Y == y)
                        return wobj;
                }                
            }
            
            return _worldObjects[index];
        }

        public void AddScreenObject(Modules.ScreenObject screenObject)
        { _screenObjects.Add(screenObject); }

        public void AddCharacterObject(Modules.WorldObject worldObject)
        { _characters.Add(worldObject); }

        public void RemoveCharacterObject(Modules.WorldObject worldObject)
        {
            Modules.Monster monster = worldObject as Modules.Monster;

            if (monster != null)
            {
                int successPercent = (int)(monster.ItemDropRate * 10);

                if (rand.Next(0, 10) <= successPercent)
                {
                    Modules.Inventory.ITEMTYPE dropType;
                    Enum.TryParse(monster.ItemDrop.ToUpper(), out dropType);

                    if (dropType != Modules.Inventory.ITEMTYPE.NONE)
                    {
                        ChangeWorldObjectVisual(ItemInfo.GetSpriteTexture(dropType),
                            monster.ObjectColor,
                            (int)monster.Position.X,
                            (int)monster.Position.Y,
                            dropType,
                            WorldMap.MAPTILETYPE.ITEMDROP,
                            monster.MonsterDungeonMap.DungeonName);
                    }
                }
            }

            _characters.Remove(worldObject);
        }

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
                int drawRadius = radius;

                if (_activeDrawLayer == 1)
                    drawRadius = 3;

                if (_activeDrawLayer == 0)
                {
                    int[] drawArray = GetAffectShape(radius, charPos, width);

                    foreach (int i in drawArray)
                    {
                        if (_worldObjects[i].DrawLayer == _activeDrawLayer)
                            _spriteBatch.Draw(_worldObjects[i].Sprite, _worldObjects[i].Position, null, _worldObjects[i].ObjectColor);
                    }
                }
                else if (_activeDrawLayer == 1)
                {
                    foreach (Modules.WorldObject wobj in _worldObjects_l1)
                    {
                        if (Utils.MathHandler.IsPointInCircle((int)wobj.Position.X / Utils.WorldMap.UNITSIZE,
                        (int)wobj.Position.Y / Utils.WorldMap.UNITSIZE,
                        (int)charPos.X / Utils.WorldMap.UNITSIZE,
                        (int)charPos.Y / Utils.WorldMap.UNITSIZE,
                        drawRadius))
                            _spriteBatch.Draw(wobj.Sprite, wobj.Position, null, wobj.ObjectColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }

                foreach (Modules.WorldObject cobj in _characters)
                {
                    if (Utils.MathHandler.IsPointInCircle((int)cobj.Position.X / Utils.WorldMap.UNITSIZE,
                        (int)cobj.Position.Y / Utils.WorldMap.UNITSIZE,
                        (int)charPos.X / Utils.WorldMap.UNITSIZE,
                        (int)charPos.Y / Utils.WorldMap.UNITSIZE,
                        drawRadius) && cobj.DrawLayer == _activeDrawLayer)
                        _spriteBatch.Draw(cobj.Sprite, cobj.Position, null, cobj.ObjectColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }

            _spriteBatch.End();
        }

        public void DrawUIString(string[] text, Vector2 offset, float[] scale, float spacing = 25, bool withSpriteBatch = true)
        {
            if (withSpriteBatch)
            {
                _spriteBatch.GraphicsDevice.Clear(Utils.GameColors.MainBackgroundColor);

                _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            }

            for (int s = 0; s < text.Length; s++)
            {
                Vector2 FontOrigin = _font.MeasureString(text[s]);
                _spriteBatch.DrawString(_font, text[s],
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + offset.X,
                    (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + (offset.Y) + (s * spacing)),
                    Color.White, 0, FontOrigin, scale[s], SpriteEffects.None, 0.5f);
            }

            if (withSpriteBatch)
                _spriteBatch.End();
        }

        private void DisplayOverworldUI(Modules.Character character, Vector2 playerOrigin)
        {
            //Show character's coordinates
            float xCenter = playerOrigin.X / Utils.WorldMap.UNITSIZE;
            float yCenter = playerOrigin.Y / Utils.WorldMap.UNITSIZE;

            //DEBUG COORDINATES
            string output = xCenter.ToString() + ", " + yCenter.ToString();
            Vector2 FontOrigin = _font.MeasureString(output) / 2;
            _spriteBatch.DrawString(_font, output, new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width / 2, 25), Color.White, 0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

            if (character.Inv.NavigatingInventory || (character.GetWeaponSprite() != character.Inv.InventorySelector.Sprite))
            {
                //Equipment slots
                //WEAPON
                _spriteBatch.Draw(character.GetWeaponSprite(),
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 880, 800),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    character.Inv.InventorySelector.Scale,
                    SpriteEffects.None,
                    0.5f);
            }

            if (character.Inv.NavigatingInventory || (character.GetHeadgearSprite() != character.Inv.InventorySelector.Sprite))
            {
                //HEADGEAR
                _spriteBatch.Draw(character.GetHeadgearSprite(),
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 880, 850),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    character.Inv.InventorySelector.Scale,
                    SpriteEffects.None,
                    0.5f);
            }

            if (character.Inv.NavigatingInventory || (character.GetFootgearSprite() != character.Inv.InventorySelector.Sprite))
            {
                //BOOTS
                _spriteBatch.Draw(character.GetFootgearSprite(),
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 880, 900),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    character.Inv.InventorySelector.Scale,
                    SpriteEffects.None,
                    0.5f);
            }

            if (character.Inv.NavigatingInventory || (character.GetChestgearSprite() != character.Inv.InventorySelector.Sprite))
            {
                //ARMOR
                _spriteBatch.Draw(character.GetChestgearSprite(),
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 880, 950),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    character.Inv.InventorySelector.Scale,
                    SpriteEffects.None,
                    0.5f);
            }

            if (character.Inv.NavigatingInventory || (character.GetAccessorySprite() != character.Inv.InventorySelector.Sprite))
            {
                //TRINKET
                _spriteBatch.Draw(character.GetAccessorySprite(),
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 880, 1000),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    character.Inv.InventorySelector.Scale,
                    SpriteEffects.None,
                    0.5f);
            }

            if (character.Inv.NavigatingInventory)
            {
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
                    Texture2D itemSprite = item.Sprite;
                    if (item.Toggled)
                        itemSprite = item.SelectedSprite;

                    _spriteBatch.Draw(itemSprite,
                        item.Position + new Vector2(_spriteBatch.GraphicsDevice.Viewport.Width, 0),
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        item.Scale,
                        SpriteEffects.None,
                        0.5f);
                }

                FontOrigin = _font.MeasureString(character.EquippedItems[0]) / 2;
                _spriteBatch.DrawString(_font, character.EquippedItems[0], new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 775, 820), Color.White, 0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

                FontOrigin = _font.MeasureString(character.EquippedItems[1]) / 2;
                _spriteBatch.DrawString(_font, character.EquippedItems[1], new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 775, 870), Color.White, 0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

                FontOrigin = _font.MeasureString(character.EquippedItems[2]) / 2;
                _spriteBatch.DrawString(_font, character.EquippedItems[2], new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 775, 920), Color.White, 0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

                FontOrigin = _font.MeasureString(character.EquippedItems[3]) / 2;
                _spriteBatch.DrawString(_font, character.EquippedItems[3], new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 775, 970), Color.White, 0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

                FontOrigin = _font.MeasureString(character.EquippedItems[4]) / 2;
                _spriteBatch.DrawString(_font, character.EquippedItems[4], new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 775, 1020), Color.White, 0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

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

                    FontOrigin = _font.MeasureString(_item.Description) / 2;
                    _spriteBatch.DrawString(_font,
                        _item.Description + "\n\nT - Trash" + "\n\nP - Place",
                        new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 800, (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + 160),
                        Color.White,
                        0,
                        FontOrigin,
                        0.75f,
                        SpriteEffects.None,
                        1f);
                }
            }
        }

        private void DisplayInteractionUI(Modules.Character character, Modules.WorldObject interactor)
        {
            Vector2 screenCenter = new Vector2(Utils.ResolutionHandler.WindowWidth / 2, Utils.ResolutionHandler.WindowHeight / 2);

            _spriteBatch.Draw(character.InteractionSprite, screenCenter + new Vector2(-300, 50), null, character.ObjectColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
            _spriteBatch.Draw((interactor as Modules.Monster).InteractionSprite, screenCenter + new Vector2(250, -150), null, interactor.ObjectColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);

            if (_showInteractionOptions)
                DrawUIString(character.InteractionOptions, new Vector2(225, 200), new float[] { 1f, 1f, 1f }, 35f, false);
        }

        private void ShowStats(Modules.Character character)
        {
            Vector2 offset = new Vector2(75, 0);

            //STAT TRACKERS
            //HP
            string health = character.Stats.HP.ToString();
            Vector2 FontOrigin = _font.MeasureString(health);
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
        }

        private void DrawUI(Vector2 characterPoint, Modules.Character character, bool world = true, Modules.WorldObject interactionObj = null)
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            ShowStats(character);

            if (world)
                DisplayOverworldUI(character, characterPoint);
            else
                DisplayInteractionUI(character, interactionObj);

            if (_showSystemUI)
            {
                Vector2 FontOrigin = _font.MeasureString(_systemMessage) / 2;
                _spriteBatch.DrawString(_font, _systemMessage,
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) + 35,
                    (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + 455),
                    Color.Black, 0, FontOrigin, 1f, SpriteEffects.None, 1f);

                _spriteBatch.Draw(_systemPopup,
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) - 235,
                    (_spriteBatch.GraphicsDevice.Viewport.Height / 2) + 400),
                    null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.1f);
            }

            _spriteBatch.End();
        }

        public void ShowSystemMessage(string message, SoundEffect sound = null)
        {
            if (sound == null)
                systemMessage.Play();
            else
                sound.Play();

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

                if (_showSysMsgTimer >= 100)
                {
                    _showSysMsgTimer = 0;
                    _showSystemUI = true;
                    _displayMsgTime = 3;
                }
            }
        }

        public void DrawUpdate(Matrix cameraMatrix, int viewRadius, Vector2 center, int mapWidth, Modules.Character player, GameTime time, ref int activeDrawLayer, bool overworld = true, Modules.WorldObject interactor = null)
        {
            DrawGame(viewRadius, center, mapWidth, cameraMatrix, overworld);
            DrawUI(center, player, overworld, interactor);

            UpdateSystemDialogs(time);

            activeDrawLayer = _activeDrawLayer;
        }
    }
}
