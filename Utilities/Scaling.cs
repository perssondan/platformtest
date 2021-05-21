using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using Windows.UI.ViewManagement;

namespace uwpPlatformer.Utilities
{
    public class Scaling
    {
        private float _scaleWidth;
        private float _scaleHeight;

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

        public Transform2DEffect ScaleBitmapKeepAspectRatio(CanvasBitmap canvasBitmap)
        {
            var scale = Math.Min(_scaleWidth, _scaleHeight);
            var image = new Transform2DEffect { Source = canvasBitmap };
            image.TransformMatrix = Matrix3x2.CreateScale(scale, scale);
            return image;
        }

        public float ScaleWidth => _scaleWidth;
        public float ScaleHeight => _scaleHeight;
        public float DesignWidth { get; set; } = 448f;
        public float DesignHeight { get; set; } = 320f;
    }
}