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
        Utils.MonsterManager _monsterManager;
        Modules.Character _character;
        Modules.Monster _interactor;

        private OrthographicCamera _camera;
        private Texture2D _mapTexture;

        private int _mapSize = 500;
        private SCENE _currentScene = SCENE.MENU;

        private bool _inOverworld = true;
        private bool _isPlayerTurn = true;
        private bool _interactionEnding = false;
        private double _timer = 0;
        private double _waitToEndInteraction = 2;

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

        private void CheckCharacterInteractions()
        {
            for (int i = 1; i < _graphicsManager.Characters.Count; i++)
            {
                if (Utils.MathHandler.WorldObjectIntersects(_graphicsManager.Characters[i], _character))
                {
                    _inOverworld = false;
                    _interactor = _graphicsManager.Characters[i] as Modules.Monster;

                    _graphicsManager.ShowSystemMessage("Encountered " + _interactor.MonsterType.ToString().ToUpper() + "!");
                    return;
                }
            }
        }

        public void LeaveInteraction()
        {
            _character.Position = new Vector2(_character.Position .X, _character.Position.Y - Utils.WorldMap.UNITSIZE);
            _inOverworld = true;
        }

        public void EndPlayerTurn(string action)
        {
            _graphicsManager.DisplayInteractionOptions(false);
            _isPlayerTurn = false;
            bool monsterDead = Utils.CombatHandler.AttackMonster(_character, ref _interactor, action, _character.GetWeapon());

            _graphicsManager.ShowSystemMessage("You " + action.ToUpper() + " " + _interactor.MonsterType.ToString().ToUpper() + "!");

            if (monsterDead)
            {
                _timer = 0;
                _interactionEnding = true;
            }
        }

        private void EndInteraction(bool escape = false)
        {
            _character.EndFight();
            LeaveInteraction();

            _graphicsManager.DisplayInteractionOptions(true);

            if (!escape)
                _graphicsManager.RemoveCharacterObject(_interactor);

            _isPlayerTurn = true;

            if (!escape)
                _graphicsManager.ShowSystemMessage("Defeated " + _interactor.MonsterType.ToString().ToUpper() + "!");
            else
                _graphicsManager.ShowSystemMessage(_interactor.MonsterType.ToString().ToUpper() + " got away!");
        }

        public void EndMonsterTurn(string action)
        {
            if (action == "runaway")
            {
                EndInteraction(true);
                return;
            }

            _graphicsManager.DisplayInteractionOptions(true);
            _isPlayerTurn = true;

            int totalDamage = Utils.CombatHandler.AttackCharacter(ref _character, _interactor, action);
            _graphicsManager.ShowSystemMessage("Took " + totalDamage.ToString() + " DAMAGE");

            _character.Stats.HP -= totalDamage;            
        }

        private void UpdateGame(GameTime gTime)
        {
            if (_currentScene == SCENE.GAME)
            {
                if (_interactionEnding)
                {
                    _timer += gTime.ElapsedGameTime.TotalSeconds;

                    if (_timer >= _waitToEndInteraction)
                    {
                        _timer = 0;
                        _interactionEnding = false;
                        EndInteraction();
                    }
                }

                _monsterManager.UpdateMonsters(gTime, _inOverworld);

                if (_isPlayerTurn)
                    _character.UpdateCharacter(gTime, _worldMap, Content, !_inOverworld);
                else if (!_isPlayerTurn && !_inOverworld && _interactor != null && !_interactionEnding)
                    _interactor.UpdateMonster(gTime, this, _graphicsManager);

                _camera.LookAt(_character.Position);

                if (_character.Stats.HP <= 0)
                    _currentScene = SCENE.DEATH;

                if (_inOverworld)
                    CheckCharacterInteractions();
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
                _graphicsManager.DrawUpdate(_camera.GetViewMatrix(), 10, _character.Position, _mapSize, _character, gameTime, _inOverworld, _interactor);
            }
            else
            {
                string _deathCause = "";

                if (_inOverworld)
                {
                    if (_character != null)
                    {
                        _deathCause = _character.GetCauseOfDeath();

                        if (_deathCause != "")
                            _deathCause = " of " + _deathCause;
                    }
                }

                _graphicsManager.DrawUIString(Utils.SceneInfo.GetSceneMessage(_currentScene, _deathCause),
                    new Vector2(500, 50),
                    Utils.SceneInfo.GetSceneScale(_currentScene));
            }
        }

        private void LoadGameContent()
        {
            _graphicsManager.ClearGraphics();
            _character = new Modules.Character("character_base", "player_interaction_img", Vector2.Zero, Content, _graphicsManager, this);
            _mapTexture = new Texture2D(GraphicsDevice, _mapSize, _mapSize);            
            _worldMap = new WorldMap(_mapSize, _mapSize, ref _mapTexture, 11, GraphicsDevice);            

            _graphicsManager.VisualizeMap(_worldMap, Content);
            _graphicsManager.AddCharacterObject(_character);            
            _character.Position = _worldMap.GetRandomGrassPoint();

            _monsterManager = new MonsterManager(Content, _graphicsManager, _worldMap);
        }
    }
}
