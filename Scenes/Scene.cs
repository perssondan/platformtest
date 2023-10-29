using GamesLibrary.Models;
using Microsoft.Graphics.Canvas;
using System;

namespace uwpPlatformer.Scenes
{
    public abstract class Scene : IScene
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public bool IsActive { get; protected set; }

        /// <inheritdoc />
        public abstract void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan);

        /// <inheritdoc />
        public virtual void Init()
        {
        }

        /// <inheritdoc />
        public abstract void Update(TimingInfo timingInfo);

        /// <inheritdoc />
        public virtual void Activate()
        {
            IsActive= true;
        }

        /// <inheritdoc />
        public virtual void Deactivate()
        {
            IsActive= false;
        }
    }
}
