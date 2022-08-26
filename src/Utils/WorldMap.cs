using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Little_Might.Utils
{
    internal class WorldMap
    {
        public struct Teleportal
        {
            public Teleportal(Vector2 portalEnter, Vector2 portalExit, string name, int enterLayer, int exitLayer, DungeonMap dMap)
            {
                PortalName = name;
                PortalEnter = portalEnter;
                PortalExit = portalExit;

                PortalDungeonMap = dMap;
                PortalEnterLayer = enterLayer;
                PortalExitLayer = exitLayer;
            }

            public string PortalName;
            public Vector2 PortalEnter;
            public Vector2 PortalExit;

            public DungeonMap PortalDungeonMap;
            public int PortalEnterLayer;
            public int PortalExitLayer;
        }

        private static Vector2 GetDungeonExitDoor(int[] dMap, Vector2 entrance, int mapWidth)
        {
            Vector2 exit = Vector2.Zero;

            if (dMap != null)
            {
                for (int i = 0; i < dMap.Length; i++)
                {
                    if (dMap[i] == 8)
                    {
                        exit = MathHandler.Get2DPoint(i, mapWidth);
                        break;
                    }
                }
            }

            return exit;
        }

        public struct LayerMapTiles
        {
            public LayerMapTiles(MAPTILETYPE t, Vector2 pos)
            {
                WorldPosition = pos;
                TileType = t;
            }

            public MAPTILETYPE TileType;
            public Vector2 WorldPosition;
        }

        public struct DungeonMap
        {
            public DungeonMap(Vector2 door, int mapSize, string name)
            {
                DungeonDoor = door;
                Map = DungeonPresets.GetDungeonMap(name);

                MapWorldPoints = new LayerMapTiles[Map.Length];
                MapWidth = mapSize;
                DungeonName = name;

                DungeonExitDoor = WorldMap.GetDungeonExitDoor(Map, door, MapWidth);
                GenerateMapWorldPoints();
            }

            public int MapWidth;
            public Vector2 DungeonDoor;
            public Vector2 DungeonExitDoor;
            public int[] Map;
            public LayerMapTiles[] MapWorldPoints;
            public string DungeonName;

            private void GenerateMapWorldPoints()
            {
                Vector2 StartPoint = new Vector2((DungeonDoor.X - DungeonExitDoor.X) * Utils.WorldMap.UNITSIZE,
                    (DungeonDoor.Y - DungeonExitDoor.Y) * Utils.WorldMap.UNITSIZE);

                Vector2 trackingPoint = StartPoint;
                int widthTracker = 0;
                for (int i = 0; i < MapWorldPoints.Length; i++)
                {
                    if (Map[i] == 0)
                    {
                        MapWorldPoints[i] = new LayerMapTiles(MAPTILETYPE.OUTOFBOUNDS, trackingPoint);
                    }
                    else if (Map[i] == 1)
                    {
                        MapWorldPoints[i] = new LayerMapTiles(MAPTILETYPE.STONE, trackingPoint);
                    }
                    else if (Map[i] == 8)
                    {
                        MapWorldPoints[i] = new LayerMapTiles(MAPTILETYPE.PRARIEDUNGEON, trackingPoint);

                        Vector2 startpoint = MathHandler.Get2DPoint(i, MapWidth);
                        
                    }

                    trackingPoint.X += UNITSIZE;
                    widthTracker++;

                    if (widthTracker >= MapWidth)
                    {
                        widthTracker = 0;
                        trackingPoint.Y += UNITSIZE;
                        trackingPoint.X = StartPoint.X;
                    }
                }
            }
        }

        public static int UNITSIZE = 9;

        private Color[] _colorMap;
        private Vector2 _startingPosition;

        private List<int> _treePoints;
        private List<int> _evergreenPoints;
        private List<int> _grassPoints;
        private List<int> _waterPoints;
        private List<int> _mountainPoints;
        private List<int> _chestPoints;
        private List<Teleportal> _dungeonPoints;
        private List<DungeonMap> _dungeonMaps;

        private int _width;
        private Random _tileRandom;

        public MAPTILETYPE[,] MapTiles;

        private int[] _chestItems = { 7, 17, 18, 19 };

        public enum MAPTILETYPE
        {
            GRASS = 0,
            WATER,
            TREE,
            BUSH,
            ROCK,
            HERBS,
            EVERGREEN,
            MOUNTAIN,
            FRUIT,
            CHEST,
            CAMPFIRE,
            ITEMDROP,
            FURNACE,
            OUTOFBOUNDS,
            PRARIEDUNGEON,
            STONE
        }

        public Teleportal GetPortalByLocation(Vector2 location, bool exiting = false)
        {
            foreach (Teleportal tele in _dungeonPoints)
            {
                if (!exiting)
                {
                    if ((int)tele.PortalEnter.X == (int)location.X / UNITSIZE &&
                        (int)tele.PortalEnter.Y == (int)location.Y / UNITSIZE)
                        return tele;
                }
                else
                {
                    if ((int)tele.PortalExit.X == (int)location.X / UNITSIZE &&
                        (int)tele.PortalExit.Y == (int)location.Y / UNITSIZE)
                        return tele;
                }
            }

            return new Teleportal(Vector2.Zero, Vector2.Zero, "", 0, 0, new DungeonMap());
        }

        public List<DungeonMap> DungeonMaps
        {
            get { return _dungeonMaps; }
        }

        public Color[] ColorMap
        {
            get { return _colorMap; }
        }

        public Vector2 StartingPosition
        {
            get { return _startingPosition; }
        }

        public int MapWidth
        {
            get { return _width; }
        }

        public WorldMap(int w, int h, ref Texture2D tex, int oct, GraphicsDevice dev)
        {
            _width = w;

            _treePoints = new List<int>();
            _evergreenPoints = new List<int>();
            _grassPoints = new List<int>();
            _waterPoints = new List<int>();
            _mountainPoints = new List<int>();
            _chestPoints = new List<int>();
            _tileRandom = new Random();
            _dungeonPoints = new List<Teleportal>();
            _dungeonMaps = new List<DungeonMap>();

            MapTiles = new MAPTILETYPE[(w * UNITSIZE), (w * UNITSIZE)];

            GenerateNoiseMap(w, h, ref tex, oct, dev);
        }

        private void GenerateNoiseMap(int width, int height, ref Texture2D noiseTexture, int octaves, GraphicsDevice device)
        {
            var data = new float[width * height];

            /// track min and max noise value. Used to normalize the result to the 0 to 1.0 range.
            var min = float.MaxValue;
            var max = float.MinValue;

            /// rebuild the permutation table to get a different noise pattern.
            /// Leave this out if you want to play with changing the number of octaves while
            /// maintaining the same overall pattern.
            Noise2d.Reseed();

            var frequency = 0.5f;
            var amplitude = 1f;

            for (var octave = 0; octave < octaves; octave++)
            {
                /// parallel loop - easy and fast.
                Parallel.For(0
                    , width * height
                    , (offset) =>
                    {
                        var i = offset % width;
                        var j = offset / width;
                        var noise = Noise2d.Noise(i * frequency * 1f / width, j * frequency * 1f / height);
                        noise = data[j * width + i] += noise * amplitude;

                        min = Math.Min(min, noise);
                        max = Math.Max(max, noise);
                    }
                );

                frequency *= 5;
                amplitude /= 2;
            }

            if (noiseTexture != null && (noiseTexture.Width != width || noiseTexture.Height != height))
            {
                noiseTexture.Dispose();
                noiseTexture = null;
            }
            if (noiseTexture == null)
            {
                noiseTexture = new Texture2D(device, width, height, false, SurfaceFormat.Color);
            }

            Random rand = new Random(8);

            int index = 0;
            var colors = data.Select(
                (f) =>
                {
                    var norm = (f - min) / (max - min);
                    Color col = new Color(norm, norm, norm, 1);

                    if (col.R < 255 && col.R >= 200)
                    {
                        _mountainPoints.Add(index);
                        col = GameColors.MountainMapColor; //mountain
                    }
                    else if (col.R < 200 && col.R >= 150)
                    {
                        _evergreenPoints.Add(index);
                        col = GameColors.EvergreenForestMapColor; //evergreen forest
                    }
                    else if (col.R < 150 && col.R >= 100)
                    {
                        _treePoints.Add(index);
                        col = GameColors.TreeForestMapColor; //tree forest

                        if (rand.Next(0, 200) >= 199)
                        {
                            col = GameColors.TreeFruitMapColor;
                        }
                    }
                    else if (col.R < 100 && col.R >= 50)
                    {
                        _grassPoints.Add(index);
                        col = GameColors.GrassMapColor; //grass

                        int grasslandObj = rand.Next(0, 300);
                        if (grasslandObj <= 1)
                        {
                            col = GameColors.RockMapColor;
                        }
                        else if (grasslandObj > 1 && grasslandObj <= 3)
                        {
                            col = GameColors.BushMapColor;
                        }
                        else if (grasslandObj > 3 && grasslandObj <= 5)
                        {
                            col = GameColors.HerbMapColor;
                        }
                    }
                    else
                    {
                        _waterPoints.Add(index);
                        col = GameColors.WaterMapColor; //water
                    }

                    index++;
                    return col;
                }
            ).ToArray();

            //Place chests
            for (int i = 0; i < (width * 0.25f); i++)
            {
                int chest = rand.Next(0, colors.Length - 1);
                colors[chest] = Color.Red;

                _chestPoints.Add(chest);
            }

            CreateDungoen("SKAME RAGH", ref colors, 28, 0, 1);
            CreateDungoen("NORLOCKE", ref colors, 20, 0, 1);
            CreateDungoen("UNDEACA", ref colors, 50, 0, 1);
            CreateDungoen("HAWKINEL", ref colors, 10, 0, 1);
            CreateDungoen("EVANSANO", ref colors, 25, 0, 1);
            CreateDungoen("THANARG", ref colors, 35, 0, 1);
            CreateDungoen("MELKOG", ref colors, 40, 0, 1);
            CreateDungoen("SARKATH", ref colors, 15, 0, 1);

            CreateDungoen("VALISHA", ref colors, 20, 0, 1);
            CreateDungoen("DORYU", ref colors, 20, 0, 1);
            CreateDungoen("KAIDA", ref colors, 20, 0, 1);
            CreateDungoen("VALISHA", ref colors, 20, 0, 1);
            CreateDungoen("VALISHA", ref colors, 20, 0, 1);
            CreateDungoen("VALISHA", ref colors, 20, 0, 1);

            _colorMap = colors;
            noiseTexture.SetData(colors);

            //Export to a file for DEBUG purposes
            Stream stream = File.Create("worldmap_file.png");
            noiseTexture.SaveAsPng(stream, noiseTexture.Width, noiseTexture.Height);
            stream.Dispose();
            noiseTexture.Dispose();
        }

        private void CreateDungoen(string dungeonName, ref Color[] mapColors, int dungeonSize, int exitLayer, int enterLayer)
        {
            int dungeonIndex = _grassPoints[Utils.MathHandler.GetRandomNumber(150, _grassPoints.Count - 1)];
            mapColors[dungeonIndex] = GameColors.PrarieDungeonMapColor;
                        
            Vector2 worldPoint = Utils.MathHandler.Get2DPoint(dungeonIndex, _width);
            DungeonMap dungeonMap = new DungeonMap(worldPoint, dungeonSize, dungeonName);

            _dungeonPoints.Add(new Teleportal(new Vector2((int)worldPoint.X, (int)worldPoint.Y),
                GetDungeonExitDoor(dungeonMap.Map, worldPoint, dungeonMap.MapWidth),
                dungeonName, enterLayer, exitLayer, dungeonMap));

            _dungeonMaps.Add(dungeonMap);
        }

        public DungeonMap GetDungeonByName(string name)
        {
            foreach (DungeonMap dMap in _dungeonMaps)
            {
                if (dMap.DungeonName == name)
                {
                    return dMap;
                }
            }

            return new DungeonMap();
        }

        public Vector2 GetRandomDungeonMapPoint(string dungeonName)
        {
            Random rand = new Random();
            foreach (DungeonMap dMap in _dungeonMaps)
            {
                MAPTILETYPE randTileType = MAPTILETYPE.OUTOFBOUNDS;

                if (dMap.DungeonName == dungeonName)
                {
                    while (randTileType == MAPTILETYPE.OUTOFBOUNDS)
                    {
                        int randIndex = rand.Next(0, dMap.MapWorldPoints.Length);
                        if (dMap.MapWorldPoints[randIndex].TileType == MAPTILETYPE.STONE)
                            return dMap.MapWorldPoints[randIndex].WorldPosition;

                        randTileType = dMap.MapWorldPoints[randIndex].TileType;
                    }
                }
            }

            return Vector2.Zero;
        }

        public Vector2 GetRandomGrassPoint()
        {
            int arrayIndex = _grassPoints[Utils.MathHandler.GetRandomNumber(150, _grassPoints.Count - 1)];

            _startingPosition = MathHandler.Get2DPoint(arrayIndex, _width) * UNITSIZE;
            return _startingPosition;
        }

        public Vector2 GetRandomForestPoint()
        {
            int arrayIndex = _treePoints[Utils.MathHandler.GetRandomNumber(150, _treePoints.Count - 1)];

            _startingPosition = MathHandler.Get2DPoint(arrayIndex, _width) * UNITSIZE;
            return _startingPosition;
        }

        public Modules.Inventory.ITEMTYPE GetBushItem()
        {
            int type = _tileRandom.Next(2, 6);

            if (type == (int)Modules.Inventory.ITEMTYPE.BERRY)
                return Modules.Inventory.ITEMTYPE.BERRY;
            else if (type == (int)Modules.Inventory.ITEMTYPE.STICK)
                return Modules.Inventory.ITEMTYPE.STICK;

            return Modules.Inventory.ITEMTYPE.NONE;
        }

        public Modules.Inventory.ITEMTYPE GetHerbItem()
        {
            int type = Utils.MathHandler.GetRandomNumber(0, 12);

            if (type == (int)Modules.Inventory.ITEMTYPE.GARLIC)
                return Modules.Inventory.ITEMTYPE.GARLIC;
            else if (type == (int)Modules.Inventory.ITEMTYPE.THYME)
                return Modules.Inventory.ITEMTYPE.THYME;
            else if (type == (int)Modules.Inventory.ITEMTYPE.OREGANO)
                return Modules.Inventory.ITEMTYPE.OREGANO;

            return Modules.Inventory.ITEMTYPE.NONE;
        }

        public Modules.Inventory.ITEMTYPE GetChestItem()
        {
            return (Modules.Inventory.ITEMTYPE)Utils.MathHandler.GetRandomNumber(0, 11);
        }

        public Modules.Inventory.ITEMTYPE GetTreeItem()
        {
            int type = _tileRandom.Next(0, 20);

            if (type == 5)
                return Modules.Inventory.ITEMTYPE.TWINE;

            return Modules.Inventory.ITEMTYPE.NONE;
        }

        public Modules.Inventory.ITEMTYPE GetStoneItem()
        {
            int type = _tileRandom.Next(3, 5);

            if (type == (int)Modules.Inventory.ITEMTYPE.FLINT)
                return Modules.Inventory.ITEMTYPE.FLINT;
            else if (type == (int)Modules.Inventory.ITEMTYPE.STONE)
                return Modules.Inventory.ITEMTYPE.STONE;

            return Modules.Inventory.ITEMTYPE.NONE;
        }

        public void ChangeTile(Vector2 pos, MAPTILETYPE tileType, DungeonMap dMap)
        {
            if (dMap.DungeonName == "" || dMap.DungeonName == null)
            {
                MapTiles[(int)pos.X, (int)pos.Y] = tileType;
            }
            else
            {
                for (int i = 0; i < dMap.MapWorldPoints.Length; i++)
                {
                    if (Utils.MathHandler.WorldObjectIntersects(dMap.MapWorldPoints[i].WorldPosition, pos))
                        dMap.MapWorldPoints[i].TileType = tileType;
                }
            }
        }

        public MAPTILETYPE GetTileType(Vector2 pos)
        {
            if (pos.X < 0 || pos.Y < 0 ||
                pos.X >= _width * UNITSIZE || pos.Y >= _width * UNITSIZE)
                return MAPTILETYPE.OUTOFBOUNDS;

            return MapTiles[(int)pos.X, (int)pos.Y];
        }

        public MAPTILETYPE GetDungeonTileType(Vector2 pos, DungeonMap dMap)
        {
            for (int i = 0; i < dMap.MapWorldPoints.Length; i++)
            {
                if (Utils.MathHandler.WorldObjectIntersects(dMap.MapWorldPoints[i].WorldPosition, pos))
                {
                    return dMap.MapWorldPoints[i].TileType;
                }
            }

            return MAPTILETYPE.OUTOFBOUNDS;
        }
    }
}
