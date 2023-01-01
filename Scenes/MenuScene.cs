using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uwpPlatformer.EventArguments;

namespace uwpPlatformer.Scenes
{
    public class MenuScene : Scene
    {
        private readonly IEventSystem _eventSystem;
        private TimeSpan? _startTime;

        public MenuScene(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public override string Name => nameof(MenuScene);

        public override void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            canvasDrawingSession.DrawText(nameof(MenuScene), new System.Numerics.Vector2(100, 100), Windows.UI.Colors.AntiqueWhite);
        }

        public override void Update(TimingInfo timingInfo)
        {
            if (_startTime is null)
            {
                _startTime = timingInfo.TotalTime;
            }

            if ((timingInfo.TotalTime - _startTime) > TimeSpan.FromSeconds(5))
            {
                _eventSystem.Send(this, new MenuSceneEvent(true));
                _startTime = null;
            }
        }
    }
}
