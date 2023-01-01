using GamesLibrary.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Models;
using uwpPlatformer.Platform;
using uwpPlatformer.Utilities;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace uwpPlatformer
{
    public sealed partial class MainPage : Page
    {
        private Scaling _scaling = new Scaling();
        private Game _game;
        private CanvasRenderTarget _offscreen;

        public MainPage()
        {
            InitializeComponent();
            Window.Current.SizeChanged += OnWindowSizeChanged;

            _scaling.SetScale();
        }

        public Game Game => _game;

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

            if (_game == null) return false;

            EnforceOffScreenCreated(canvasDevice, 32*14, 32*10);// _game.World.WorldPixelWidth, _game.World.WorldPixelHeight);
            if (_offscreen == null) return false;

            using (var drawingSession = _offscreen.CreateDrawingSession())
            {
                drawingSession.Clear(Colors.DarkBlue);
                _game.Draw(drawingSession, elapsedTime);
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
                _game?.Update(new TimingInfo(args.Timing.ElapsedTime, args.Timing.TotalTime));
            }
        }

        private long _count = 0;
        private double _elapsed = 0d;

        private void OnGameCanvasCreateResources(ICanvasAnimatedControl canvasControl, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(InitGameAssetsAsync(canvasControl).AsAsyncAction());
        }

        private async Task InitGameAssetsAsync(ICanvasAnimatedControl canvasControl)
        {
            if (_game is null)
            {
                _game = new Game(canvasControl);
                await _game.InitAsync();
            }
        }

        private void OnGameCanvasTapped(object sender, TappedRoutedEventArgs args)
        {
        }
    }
}