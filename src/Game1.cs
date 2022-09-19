using Little_Might.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Little_Might
{
    public class Game1 : Game
    {
        private Utils.GraphicsManager _graphicsManager;
        private Utils.WorldMap _worldMap;
        private Utils.MonsterManager _monsterManager;
        private Modules.Character _character;
        private Modules.Monster _interactor;

        private OrthographicCamera _camera;
        private Texture2D _mapTexture;

        private int _mapSize = 500;
        private SCENE _currentScene = SCENE.MENU;

        private bool _inOverworld = true;
        private bool _isPlayerTurn = true;
        private bool _interactionEnding = false;
        private double _timer = 0;
        private double _waitToEndInteraction = 2;
        private int _drawLayer = 0;
        private int _lastDrawLayer = 0;

        private bool overworldMusic = false;
        private bool menuMusic = false;

        private SoundEffect deathSoundfx;
        private SoundEffect menuSelectSound;

        private SoundEffect mainMenuSound;
        private SoundEffectInstance mainMenuSoundInstance;

        private SoundEffect overworld_1Sound;
        private SoundEffectInstance overworldSoundInstance;

        private SoundEffect battle_1Sound;
        private SoundEffectInstance battleSoundInstance;

        private SoundEffect dungeonSound;
        private SoundEffectInstance dungeonSoundInstance;

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

            deathSoundfx = Content.Load<SoundEffect>("sound/death");
            menuSelectSound = Content.Load<SoundEffect>("sound/menuselect");
            overworld_1Sound = Content.Load<SoundEffect>("sound/overworld_2");
            battle_1Sound = Content.Load<SoundEffect>("sound/battle_1");
            mainMenuSound = Content.Load<SoundEffect>("sound/mainmenu");
            dungeonSound = Content.Load<SoundEffect>("sound/dungeon");

            overworldSoundInstance = overworld_1Sound.CreateInstance();
            overworldSoundInstance.IsLooped = true;

            mainMenuSoundInstance = mainMenuSound.CreateInstance();
            mainMenuSoundInstance.IsLooped = true;

            battleSoundInstance = battle_1Sound.CreateInstance();
            battleSoundInstance.IsLooped = true;

            dungeonSoundInstance = dungeonSound.CreateInstance();
            dungeonSoundInstance.IsLooped = true;
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
                //If the characters intersect AND they are on the same draw layer, then we can begin the encounter
                if (Utils.MathHandler.WorldObjectIntersects(_graphicsManager.Characters[i], _character) &&
                    _graphicsManager.Characters[i].DrawLayer == _character.DrawLayer)
                {
                    _inOverworld = false;
                    _interactor = _graphicsManager.Characters[i] as Modules.Monster;

                    string monsterName = _interactor.MonsterType.ToString().ToUpper();

                    if (monsterName == "CELESTIALHORROR")
                        monsterName = "???";

                    _graphicsManager.ShowSystemMessage("Encountered " + monsterName + "!");
                    return;
                }
            }
        }

        public void LeaveInteraction()
        {
            _character.Position = new Vector2(_character.Position.X, _character.Position.Y - Utils.WorldMap.UNITSIZE);
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

            if (_interactor == null)
                return;

            _graphicsManager.DisplayInteractionOptions(true);

            if (!escape)
                _graphicsManager.RemoveCharacterObject(_interactor);

            _isPlayerTurn = true;

            if (!escape)
                _graphicsManager.ShowSystemMessage("Defeated " + _interactor.MonsterType.ToString().ToUpper() + "!", deathSoundfx);
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

            int totalDamage = Utils.CombatHandler.AttackCharacter(ref _character, _interactor, action) - (_character.GetEquipModifiers());
            _graphicsManager.ShowSystemMessage("Took " + totalDamage.ToString() + " DAMAGE");

            _character.Stats.HP -= totalDamage;
        }

        private void UpdateGame(GameTime gTime)
        {
            if (_currentScene != SCENE.GAME && !menuMusic)
            {
                overworldSoundInstance.Stop(true);
                battleSoundInstance.Stop(true);

                menuMusic = true;
                mainMenuSoundInstance.Play();
            }

            if (_currentScene == SCENE.GAME)
            {
                if (menuMusic)
                {
                    mainMenuSoundInstance.Stop(true);
                    menuMusic = false;
                }

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
                {
                    _timer = 0;
                    _interactionEnding = false;
                    EndInteraction(true);

                    _currentScene = SCENE.DEATH;
                    overworldMusic = false;
                    _drawLayer = 0;
                }

                if (_inOverworld)
                {
                    CheckCharacterInteractions();

                    if (!overworldMusic)
                    {
                        battleSoundInstance.Stop(true);
                        overworldMusic = true;

                        if (_drawLayer == 0)
                        {
                            overworldSoundInstance.Play();
                            dungeonSoundInstance.Stop();
                        }
                        else if (_drawLayer == 1)
                        {
                            dungeonSoundInstance.Play();
                            overworldSoundInstance.Pause();
                        }
                    }
                }
                else if (overworldMusic && !_inOverworld)
                {
                    overworldMusic = false;

                    if (_drawLayer == 0)
                    {
                        overworldSoundInstance.Pause();
                    }
                    else if (_drawLayer == 1)
                    {
                        dungeonSoundInstance.Pause();
                    }

                    battleSoundInstance.Play();
                }
            }
            else if (_currentScene == SCENE.DIFFICULTY)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D1) || Keyboard.GetState().IsKeyDown(Keys.NumPad1))
                {
                    menuSelectSound.Play(0.5f, -0.5f, 0f);

                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D2) || Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                {
                    menuSelectSound.Play(0.5f, -0.5f, 0f);

                    _mapSize *= 2;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D3) || Keyboard.GetState().IsKeyDown(Keys.NumPad3))
                {
                    menuSelectSound.Play(0.5f, -0.5f, 0f);

                    _mapSize *= 4;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D4) || Keyboard.GetState().IsKeyDown(Keys.NumPad4))
                {
                    menuSelectSound.Play(0.5f, -0.5f, 0f);

                    _mapSize *= 6;
                    LoadGameContent();
                    _currentScene = SCENE.GAME;
                }
            }
            else if (_currentScene == SCENE.MENU)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    menuSelectSound.Play(0.5f, -0.5f, 0f);
                    _currentScene = SCENE.DIFFICULTY;
                }
            }
            else if (_currentScene == SCENE.DEATH)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    _graphicsManager.SetDrawLayer(0);
                    menuSelectSound.Play(0.5f, -0.5f, 0f);
                    _currentScene = SCENE.MENU;

                    overworldMusic = false;
                    _inOverworld = true;
                }
            }

            _lastDrawLayer = _drawLayer;
        }

        private void DrawGame(GameTime gameTime)
        {
            if (_currentScene == SCENE.GAME)
            {
                _graphicsManager.DrawUpdate(_camera.GetViewMatrix(), _character.FieldOfView, _character.Position, _mapSize, _character, gameTime, ref _drawLayer, _inOverworld, _interactor);

                if (_lastDrawLayer != _drawLayer)
                {
                    overworldMusic = false;
                }
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

            _graphicsManager.VisualizeWorld(_worldMap, Content);
            _graphicsManager.AddCharacterObject(_character);
            _character.Position = _worldMap.GetRandomGrassPoint();

            _monsterManager = new MonsterManager(Content, _graphicsManager, _worldMap);
        }
    }
}
