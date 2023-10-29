using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using uwpPlatformer.EventArguments;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Platform;
using uwpPlatformer.Systems;

namespace uwpPlatformer.Scenes
{
    public class MenuScene : Scene
    {
        private readonly IGameAssetsProvider _gameAssetsProvider;
        private readonly IGameObjectManager _gameObjectManager;
        private readonly IEventSystem _eventSystem;
        private readonly World _world;
        private readonly GraphicsSystem _graphicsSystem;
        private TimeSpan _startTime = TimeSpan.Zero;

        public MenuScene(IGameAssetsProvider gameAssetsProvider, IEventSystem eventSystem)
        {
            _gameAssetsProvider = gameAssetsProvider;
            _eventSystem = eventSystem;

            _gameObjectManager = new GameObjectManager();

            _world = new World(_gameAssetsProvider, _gameObjectManager);
            _graphicsSystem = new GraphicsSystem(_gameObjectManager);
        }

        public override string Name => nameof(MenuScene);

        private TimeSpan TimePassed { get; set; }

        public override void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
        }

        public override void Update(TimingInfo timingInfo)
        {
            if (IsFinished(timingInfo))
            {
                _eventSystem.Send(this, new MenuSceneEvent(true));
            }

            _gameObjectManager.Update();
            _graphicsSystem.Update(timingInfo);
        }

        private bool IsFinished(TimingInfo timingInfo)
        {
            if (_startTime == TimeSpan.Zero)
            {
                _startTime = timingInfo.TotalTime;
            }

            TimePassed = timingInfo.TotalTime - _startTime;
            if (TimePassed > TimeSpan.FromSeconds(5))
            {
                _startTime = TimeSpan.Zero;
                return true;
            }

            return false;
        }

        public override void Init()
        {
            _graphicsSystem.Init();
            base.Init();
        }
    }
}
