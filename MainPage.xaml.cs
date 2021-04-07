using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using uwpKarate.GameObjects;
using uwpKarate.Models;
using uwpKarate.Utilities;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace uwpKarate
{
    public sealed partial class MainPage : Page
    {
        private CanvasBitmap _backgroundBitmap;
        private CanvasBitmap _tileAtlas;
        private Scaling _scaling = new Scaling();
        private GameStateManager _gameStateManager = new GameStateManager();
        private World _world;

        public MainPage()
        {
            InitializeComponent();
            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            _scaling.SetScale();
        }

        private void OnGameCanvasDraw(ICanvasAnimatedControl canvasControl, CanvasAnimatedDrawEventArgs args)
        {
            //var image = _scaling.ScaleBitmap(_backgroundBitmap);
            _world?.Draw(args.DrawingSession);
            //    args.DrawingSession.DrawImage(image);
            //args.DrawingSession.DrawImage(image, new System.Numerics.Vector2(0f, 0f));

            args.DrawingSession.DrawText($"GameCount: {_count}, elapsed: {_elapsed}", new System.Numerics.Vector2(10, 10), Colors.AliceBlue);
            // TODO: Do we need this?
            GameCanvas.Invalidate();
        }

        private void OnGameCanvasUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            _count = args.Timing.UpdateCount;
            _elapsed = args.Timing.ElapsedTime.TotalMilliseconds;
            if (!sender.Paused)
            {
                _world?.Update(args.Timing.ElapsedTime);
            }
        }

        private long _count = 0;
        private double _elapsed = 0d;

        private void OnGameCanvasCreateResources(ICanvasAnimatedControl canvasControl, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourceAsync(canvasControl).AsAsyncAction());
        }

        private async Task CreateResourceAsync(ICanvasAnimatedControl canvasControl)
        {
            _backgroundBitmap = await CanvasBitmap.LoadAsync(canvasControl, new Uri("ms-appx:///Assets/GameAssets/images/ikplusbakdrop.png"));
            _tileAtlas = await CanvasBitmap.LoadAsync(canvasControl, new Uri("ms-appx:///Assets/GameAssets/images/tiles.png"));

            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/GameAssets/images/mytilemap.json"));

            var jsonSerializer = new JsonSerializer();
            using (var randomAccessStream = await storageFile.OpenReadAsync())
            using (var streamReader = new StreamReader(randomAccessStream.AsStream()))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var map = jsonSerializer.Deserialize<Map>(jsonTextReader);
                _world = new World(_tileAtlas, map);
            }
        }

        private void OnGameCanvasTapped(object sender, TappedRoutedEventArgs args)
        {
        }
    }
}