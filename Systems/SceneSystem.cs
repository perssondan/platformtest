using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using uwpPlatformer.Platform;
using uwpPlatformer.Scenes;
using uwpPlatformer.Utilities;

namespace uwpPlatformer.Systems
{
    public class SceneSystem : ISystem
    {
        private IDictionary<string, IScene> _scenes = new Dictionary<string, IScene>();
        private IEventSystem _eventSystem;
        private IScene _scene;
        private StateMachine<GameTrigger, GameState> _stateMachine;

        public SceneSystem(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _stateMachine = new StateMachine<GameTrigger, GameState>(GameState.Splash);
            _stateMachine.OnStateChanged += (trigger, fromState, toState) =>
            {
                UpdateActiveScene();
            };
            InitializeStateMachine();
        }

        public string Name => nameof(SceneSystem);

        public GameState State => _stateMachine.State;

        public void Init()
        {
            UpdateActiveScene();
        }

        public void AddMenuScene(IGameAssetsProvider gameAssetsProvider)
        {
            _scenes.Add(nameof(MenuScene), new MenuScene(gameAssetsProvider, _eventSystem));
        }

        public void AddSplashScene(IGameAssetsProvider gameAssetsProvider)
        {
            _scenes.Add(nameof(SplashScene), new SplashScene(gameAssetsProvider, _eventSystem));
        }

        public void AddLevelOneScene(IGameAssetsProvider gameAssetsProvider)
        {
            _scenes.Add(nameof(LevelOneScene), new LevelOneScene(gameAssetsProvider));
        }

        public void Update(TimingInfo timingInfo)
        {
            _scene?.Update(timingInfo);
        }

        public void FireTrigger(GameTrigger gameTrigger)
        {
            _stateMachine.FireTrigger(gameTrigger);
        }

        private void InitializeStateMachine()
        {
            _stateMachine.ConfigureState(GameState.Splash, GameTrigger.SplashShown, GameState.Menu);
            _stateMachine.ConfigureState(GameState.Menu, GameTrigger.StartGame, GameState.GamePlay);
            _stateMachine.ConfigureState(GameState.GamePlay, GameTrigger.GameOver, GameState.GameOver);
            _stateMachine.ConfigureState(GameState.GameOver, GameTrigger.ActivateMenu, GameState.Menu);
        }

        internal void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _scene?.Draw(canvasDrawingSession, timeSpan);
        }

        private void UpdateActiveScene()
        {
            _scene?.Deactivate();
            switch (State)
            {
                case GameState.Splash:
                    _scene = _scenes[nameof(SplashScene)];
                    _scene.Init();
                    break;
                case GameState.Menu:
                    _scene = _scenes[nameof(MenuScene)];
                    _scene.Init();
                    break;
                case GameState.Loading:
                    _scene = _scenes[nameof(LoadingScene)];
                    _scene.Init();
                    break;
                case GameState.GamePlay:
                    _scene = _scenes[nameof(LevelOneScene)];
                    _scene.Init();
                    break;
                case GameState.GamePlayMenu:
                    //_scene = _scenes[nameof(GamePlayScene)];
                    break;
                case GameState.GameOver:
                    _scene = _scenes[nameof(GameOverScene)];
                    break;
            }

            _scene?.Activate();
        }

        public enum GameState
        {
            Splash, // stay here for a while. Triggers to goto next state, Delay time threshold
            Menu, // shows menu until user selects. Trigger, user selection. Start game, settings, Exit game
            Loading, // Load game assets and other stuff. Trigger, stuff loaded
            GamePlay, // Lets play.
            GamePlayMenu, // Resume, Exit current game etc
            GameOver, // Lets not play
        }

        public enum GameTrigger
        {
            SplashShown,
            StartGame,
            GameOver,
            ActivateMenu,
        }
    }
}
