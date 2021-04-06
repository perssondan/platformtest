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
            _world?.Update(args.DrawingSession);
            //    args.DrawingSession.DrawImage(image);
            //args.DrawingSession.DrawImage(image, new System.Numerics.Vector2(0f, 0f));
            GameCanvas.Invalidate();
        }

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