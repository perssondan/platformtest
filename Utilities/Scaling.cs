using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI.ViewManagement;

namespace uwpKarate.Utilities
{
    public class Scaling
    {
        private float _scaleWidth;
        private float _scaleHeight;
        private readonly float DesignWidth = 640;
        private readonly float DesignHeight = 400;

        public void SetScale()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            _scaleWidth = (float)bounds.Width / DesignWidth;
            _scaleHeight = (float)bounds.Height / DesignHeight;
        }

        public Transform2DEffect ScaleBitmap(CanvasBitmap canvasBitmap)
        {
            var image = new Transform2DEffect { Source = canvasBitmap };
            image.TransformMatrix = Matrix3x2.CreateScale(_scaleWidth, _scaleHeight);
            return image;
        }

        public float ScaleWidth => _scaleWidth;
        public float ScaleHeight => _scaleHeight;
    }
}