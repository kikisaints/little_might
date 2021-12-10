using Little_Might.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Little_Might
{
    public class Game1 : Game
    {
        Utils.GraphicsManager _graphicsManager;
        Utils.WorldMap _worldMap;
        Modules.Character _character;

        private OrthographicCamera _camera;
        private Texture2D _mapTexture;

        private int _mapSize = 500;
        private SCENE _currentScene = SCENE.MENU;

        public enum SCENE
        {
            MENU = 0,
            DIFFICULTY,
            GAME,
            DEATH,
            CREDITS,
            OPTIONS
        }        

        public Game1()
        {
            _graphicsManager = new Utils.GraphicsManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 400, 240);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        protected override void LoadContent()
        {
            _graphicsManager.Load(GraphicsDevice, Content, this.Window);
            ResolutionHandler.ChangeResolution(_graphicsManager.Graphics, 1920, 1080);            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)
                || Keyboard.GetState().IsKeyDown(Keys.F))
                Exit();

            UpdateGame(gameTime);

            base.Update(gameTime);            
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawGame(gameTime);
            base.Draw(gameTime);            
        }

        private void UpdateGame(GameTime gTime)
        {
            if (_currentScene == SCENE.GAME)
            {
                _character.UpdateCharacter(gTime, _worldMap, Content);
                _camera.LookAt(_character.Position);

                if (_character.Stats.HP <= 0)
                    _currentScene = SCENE.DEATH;
            }
        }

        private void DrawGame(GameTime gameTime)
        {
            if (_currentScene == SCENE.GAME)
                _graphicsManager.DrawUpdate(_camera.GetViewMatrix(), 10, _character.Position, _mapSize, _character, gameTime);

            if (_currentScene == SCENE.DIFFICULTY)
            {
                string[] menuMessage = {
                    "SELECT DIFFICULTY",
                    "1 - Baby's first survival game",
                    "2 - I play games",
                    "3 - Normal",
                    "4 - Challenge me!"
                };

                float[] messageScale = {
                    2f,
                    1f,
                    1f,
                    1f,
                    1f,
                };

                _graphicsManager.DrawUIString(menuMessage, new Vector2(500, 50), messageScale);

                if (Keyboard.GetState().IsKeyDown(Keys.D1))
                {
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D2))
                {
                    _mapSize *= 2;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    _mapSize *= 4;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D4))
                {
                    _mapSize *= 6;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
            }

            if (_currentScene == SCENE.MENU)
            {
                string[] menuMessage = {
                    "LITTLE MIGHT",
                    "press ENTER to start",
                };

                float[] messageScale = {
                    2f,
                    1f,
                };

                _graphicsManager.DrawUIString(menuMessage, new Vector2(500, 25), messageScale);

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    _currentScene = SCENE.DIFFICULTY;
                }
            }
            else if (_currentScene == SCENE.DEATH)
            {
                string causeOfDeath = _character.GetCauseOfDeath();

                if (causeOfDeath != "")
                    causeOfDeath = " of " + causeOfDeath;

                string[] deathMessage = {
                    "YOU DIED" + causeOfDeath,
                    "Press F to Rage Quit",
                    "Press R to Restart"
                };

                float[] messageScale = {
                    2f,
                    1f,
                    1f
                };

                _graphicsManager.DrawUIString(deathMessage, new Vector2(500, 25), messageScale);

                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    _currentScene = SCENE.MENU;
                }
            }
        }

        private void LoadGameContent()
        {
            _graphicsManager.ClearGraphics();

            _mapTexture = new Texture2D(GraphicsDevice, _mapSize, _mapSize);
            _character = new Modules.Character("character_base", Vector2.Zero, Content, _graphicsManager);
            _worldMap = new WorldMap(_mapSize, _mapSize, ref _mapTexture, 11, GraphicsDevice);

            _graphicsManager.VisualizeMap(_worldMap, Content, _mapSize);
            _graphicsManager.AddCharacterObject(_character);

            _character.Position = _worldMap.GetCharacterStartingPoint();
        }
    }
}
