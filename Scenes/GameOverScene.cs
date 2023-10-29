using GamesLibrary.Models;
using Microsoft.Graphics.Canvas;
using System;

namespace uwpPlatformer.Scenes
{
    public class GameOverScene : Scene
    {
        public override string Name => nameof(GameOverScene);

        public override void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            canvasDrawingSession.DrawText(Name, new System.Numerics.Vector2(100, 100), Windows.UI.Colors.AntiqueWhite);
        }

        public override void Update(TimingInfo timingInfo)
        {
        }
    }
}
