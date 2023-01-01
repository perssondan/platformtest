using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using uwpPlatformer.EventArguments;
using uwpPlatformer.Platform;
using uwpPlatformer.Systems;

namespace uwpPlatformer.GameObjects
{
    public class Game : INotifyPropertyChanged
    {
        private readonly IEventSystem _eventSystem = new EventSystem();
        private readonly SceneSystem _sceneSystem;
        private readonly ICanvasAnimatedControl _canvasControl;
        private bool _isLoading = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public Game(ICanvasAnimatedControl canvasControl)
        {
            _sceneSystem = new SceneSystem(_eventSystem);
            _canvasControl = canvasControl;

            _eventSystem.Subscribe<SplashSceneEvent>(this, (sender, splashSceneEvent) =>
            {
                _sceneSystem.FireTrigger(SceneSystem.GameTrigger.SplashShown);
            });

            _eventSystem.Subscribe<MenuSceneEvent>(this, (sender, menuSceneEvent) =>
            {
                _sceneSystem.FireTrigger(SceneSystem.GameTrigger.StartGame);
            });
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        public async Task InitAsync()
        {
            try
            {
                var gameAssetsProvider = new GameAssetsProvider(_canvasControl, "ms-appx:///Assets/GameAssets/images", "mytilemap.json");
                var splashAssetsProvider = new GameAssetsProvider(_canvasControl, "ms-appx:///Assets/GameAssets/images", "splashscreenmap.json");
                await gameAssetsProvider.LoadAssetsAsync();
                await splashAssetsProvider.LoadAssetsAsync();

                _sceneSystem.AddSplashScene(splashAssetsProvider);
                _sceneSystem.AddPlatformScene(gameAssetsProvider);
                _sceneSystem.Init();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void Update(TimingInfo timingInfo)
        {
            _sceneSystem.Update(timingInfo);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _sceneSystem.Draw(canvasDrawingSession, timeSpan);
        }
    }
}
