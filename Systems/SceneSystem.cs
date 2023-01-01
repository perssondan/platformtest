using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using uwpPlatformer.Platform;
using uwpPlatformer.Scenes;
using static uwpPlatformer.Systems.SceneSystem;

namespace uwpPlatformer.Systems
{
    public class SceneSystem : ISystem
    {
        private Dictionary<GameTrigger, List<Func<GameTrigger, GameState, GameState?>>> _allowedTransitions = new Dictionary<GameTrigger, List<Func<GameTrigger, GameState, GameState?>>>();
        private IDictionary<string, IScene> _scenes = new Dictionary<string, IScene>();
        private IEventSystem _eventSystem;
        private IScene _scene;
        private GameState _state;

        public SceneSystem(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            AddTransitions();
        }

        public string Name => nameof(SceneSystem);

        public GameState State
        {
            get => _state;
            private set
            {
                _state = value;
                UpdateActiveScene();
            }
        }

        public void Init()
        {
            _scenes.Add(nameof(MenuScene), new MenuScene(_eventSystem));
            if (_scenes.TryGetValue(nameof(SplashScene), out var scene))
            {
                scene.Init();
            }

            _scene = scene;
        }

        public void AddSplashScene(IGameAssetsProvider gameAssetsProvider)
        {
            _scenes.Add(nameof(SplashScene), new SplashScene(gameAssetsProvider, _eventSystem));
        }

        public void AddPlatformScene(IGameAssetsProvider gameAssetsProvider)
        {
            _scenes.Add(nameof(PlatformScene), new PlatformScene(gameAssetsProvider));
        }

        public void Update(TimingInfo timingInfo)
        {
            _scene?.Update(timingInfo);
        }

        public void FireTrigger(GameTrigger gameTrigger)
        {
            if (!_allowedTransitions.TryGetValue(gameTrigger, out var transitions))
            {
                Debug.WriteLine($"{gameTrigger} has no configured transition");
                return;
            }

            var nextState = transitions
                .Select(x => x.Invoke(gameTrigger, _state))
                .Where(x => x != null)
                .SingleOrDefault();

            if (!nextState.HasValue)
            {
                Debug.WriteLine($"{gameTrigger} has no configured transition");
                return;
            }

            State = nextState.Value;
        }

        private void AddTransitions()
        {
            CreateTransition(GameTrigger.SplashShown, GameState.Splash, GameState.Menu);
            CreateTransition(GameTrigger.StartGame, GameState.Menu, GameState.GamePlay);
            CreateTransition(GameTrigger.GameOver, GameState.GamePlay, GameState.GameOver);
            CreateTransition(GameTrigger.ActivateMenu, GameState.GameOver, GameState.Menu);
        }

        private void CreateTransition(GameTrigger trigger, GameState fromState, GameState toState)
        {
            if (!_allowedTransitions.TryGetValue(trigger, out var transitions))
            {
                _allowedTransitions[trigger] = new List<Func<GameTrigger, GameState, GameState?>>();
            }
            var func = new Func<GameTrigger, GameState, GameState?>((firedTrigger, currentState) =>
            {
                if (firedTrigger != trigger) throw new InvalidOperationException("Missmatch firedTrigger and trigger! Pfff coders...");
                if (currentState != fromState)
                {
                    Debug.WriteLine($"Transition from {currentState} to {toState} with {firedTrigger} not allowed!");
                    return default;
                }

                return toState;
            });
            _allowedTransitions[trigger].Add(func);
        }

        internal void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _scene?.Draw(canvasDrawingSession, timeSpan);
        }

        private void UpdateActiveScene()
        {
            switch (_state)
            {
                case GameState.Splash:
                    _scene = _scenes[nameof(SplashScene)];
                    break;
                case GameState.Menu:
                    _scene = _scenes[nameof(MenuScene)];
                    break;
                case GameState.Loading:
                    _scene = _scenes[nameof(LoadingScene)];
                    break;
                case GameState.GamePlay:
                    _scene = _scenes[nameof(PlatformScene)];
                    _scene.Init();
                    break;
                case GameState.GamePlayMenu:
                    //_scene = _scenes[nameof(GamePlayScene)];
                    break;
                case GameState.GameOver:
                    _scene = _scenes[nameof(GameOverScene)];
                    break;
            }
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
