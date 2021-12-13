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

        private bool _inOverworld = true;

        public enum SCENE
        {
            MENU = 0,
            DIFFICULTY,
            GAME,
            BATTLE,
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
            else if (_currentScene == SCENE.DIFFICULTY)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D1) || Keyboard.GetState().IsKeyDown(Keys.NumPad1))
                {
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D2) || Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                {
                    _mapSize *= 2;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D3) || Keyboard.GetState().IsKeyDown(Keys.NumPad3))
                {
                    _mapSize *= 4;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D4) || Keyboard.GetState().IsKeyDown(Keys.NumPad4))
                {
                    _mapSize *= 6;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
            }
            else if (_currentScene == SCENE.MENU)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    _currentScene = SCENE.DIFFICULTY;
                }
            }
            else if (_currentScene == SCENE.DEATH)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    _currentScene = SCENE.MENU;
                }
            }
        }

        private void DrawGame(GameTime gameTime)
        {
            if (_currentScene == SCENE.GAME)
            {
                _graphicsManager.DrawUpdate(_camera.GetViewMatrix(), 10, _character.Position, _mapSize, _character, gameTime, _inOverworld);
            }
            else
            {
                string _deathCause = "";

                if (_character != null)
                {
                    _deathCause = _character.GetCauseOfDeath();

                    if (_deathCause != "")
                        _deathCause = " of " + _deathCause;
                }

                _graphicsManager.DrawUIString(Utils.SceneInfo.GetSceneMessage(_currentScene, _deathCause),
                    new Vector2(500, 50),
                    Utils.SceneInfo.GetSceneScale(_currentScene));
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
