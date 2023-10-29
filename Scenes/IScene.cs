using GamesLibrary.Models;
using Microsoft.Graphics.Canvas;
using System;

namespace uwpPlatformer.Scenes
{
    public interface IScene
    {
        /// <summary>
        /// Gets the name of the scene
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets if the scene is active
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="timingInfo"></param>
        void Update(TimingInfo timingInfo);

        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="canvasDrawingSession"></param>
        /// <param name="timeSpan"></param>
        void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan);

        /// <summary>
        /// Init method
        /// </summary>
        void Init();

        /// <summary>
        /// Activates the scene
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates the scene
        /// </summary>
        void Deactivate();
    }
}
