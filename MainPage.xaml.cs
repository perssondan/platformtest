﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using uwpKarate.GameObjects;
using uwpKarate.Models;
using uwpKarate.Utilities;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace uwpKarate
{
    public sealed partial class MainPage : Page
    {
        private CanvasBitmap _tileAtlasBitmap;
        private Scaling _scaling = new Scaling();
        private GameStateManager _gameStateManager = new GameStateManager();
        private World _world;
        private CanvasRenderTarget _offscreen;

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
            try
            {
                args.DrawingSession.Clear(Colors.Black);

                if (TryGetRenderedOffscreen(args.DrawingSession.Device, args.Timing.ElapsedTime, out var renderedBitmap))
                {
                    var effect = _scaling.ScaleBitmapKeepAspectRatio(renderedBitmap);
                    args.DrawingSession.DrawImage(effect);
                }
            }
            finally
            {
                GameCanvas.Invalidate();
            }

            args.DrawingSession.DrawText($"GameCount: {_count}, elapsed: {_elapsed}", new System.Numerics.Vector2(10, 10), Colors.AliceBlue);
        }

        private bool TryGetRenderedOffscreen(CanvasDevice canvasDevice, TimeSpan elapsedTime, out CanvasRenderTarget canvasRenderTarget)
        {
            canvasRenderTarget = null;

            if (_world == null) return false;

            EnforceOffScreenCreated(canvasDevice, _world.WorldPixelWidth, _world.WorldPixelHeight);
            if (_offscreen == null) return false;

            using (var drawingSession = _offscreen.CreateDrawingSession())
            {
                drawingSession.Clear(Colors.Black);
                _world.Draw(drawingSession, elapsedTime);
            }

            canvasRenderTarget = _offscreen;

            return true;
        }

        private void EnforceOffScreenCreated(CanvasDevice canvasDevice, float width, float height)
        {
            if (_offscreen != null)
                return;

            _offscreen = new CanvasRenderTarget(canvasDevice, width, height, dpi: 96);

            _scaling.DesignWidth = width;
            _scaling.DesignHeight = height;

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
            var tiledLoader = new TiledLoader();
            var map = await tiledLoader.LoadResourceAsync<Map>(new Uri("ms-appx:///Assets/GameAssets/images/mytilemap.json"));

            var tileAtlases = await Task.WhenAll(map.TileSets
                .Select(tileSet => tiledLoader.LoadResourceAsync<TileAtlas>(new Uri($"ms-appx:///Assets/GameAssets/images/{tileSet.Source}"))));

            var bitmaps = await Task.WhenAll(tileAtlases
                .Select(tileAtlas => CanvasBitmap.LoadAsync(canvasControl, new Uri($"ms-appx:///Assets/GameAssets/images/{tileAtlas.ImageSource}")).AsTask()));

            if (_world == null)
            {
                _world = new World(bitmaps, map, tileAtlases, Window.Current);
            }

            //CanvasDevice device = CanvasDevice.GetSharedDevice();
            //_offscreen = new CanvasRenderTarget(device, 320, 448, 96);
        }

        private void OnGameCanvasTapped(object sender, TappedRoutedEventArgs args)
        {
        }
    }
}