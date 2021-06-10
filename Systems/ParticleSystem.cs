using GamesLibrary.Models;
using GamesLibrary.Utilities;
using System;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.Components.Particles;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;
using Windows.UI;

namespace uwpPlatformer.Systems
{
    public class ParticleSystem : SystemBase<ParticleSystem>
    {
        private readonly IGameObjectManager _gameObjectManager;

        public ParticleSystem(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public override void Update(TimingInfo timingInfo)
        {
            // Make a copy of the list before thus not to change the list we're iterating
            var particles = _gameObjectManager.GameObjects
                .Where(gameObject => gameObject.Has<ParticleComponent>())
                .Select(gameObject => gameObject.GetComponent<ParticleComponent>())
                .ToArray();

            var now = timingInfo.TotalTime;
            foreach (var particle in particles)
            {
                var elapsedTime = now - particle.CreatedAt;

                if (elapsedTime < particle.TimeToLive)
                {
                    var progress = CalculateProgress(particle, elapsedTime);
                    UpdateParticleColor(particle, progress);
                    UpdateOpacity(particle, progress);
                    continue;
                }

                _gameObjectManager.DestroyGameObject(particle.GameObject);
            }
        }

        private ProgressStatus CalculateProgress(ParticleComponent particle, TimeSpan elapsedTime)
        {
            var ticksDifference = Math.Clamp(particle.TimeToLive.Ticks - elapsedTime.Ticks, 0f, particle.TimeToLive.Ticks);
            var startPercentage = ticksDifference / particle.TimeToLive.Ticks;
            var endPercentage = 1f - startPercentage;
            return new ProgressStatus { startPercentage = startPercentage, endPercentage = endPercentage };
        }

        private void UpdateParticleColor(ParticleComponent particle, ProgressStatus progressStatus)
        {
            var graphicsComponent = particle.GameObject.GetComponent<ShapeGraphicsComponent>();
            if (graphicsComponent is null) return;

            var nextColor = particle.StartColor.Lerp(particle.EndColor, progressStatus.endPercentage);
            graphicsComponent.Color = nextColor;
        }

        private void UpdateOpacity(ParticleComponent particle, ProgressStatus progressStatus)
        {
            if (particle.FadeBehavior == FadeBehavior.None) return;

            var graphicsComponent = particle.GameObject.GetComponent<ShapeGraphicsComponent>();
            if (graphicsComponent is null) return;

            var alpha = 0f;
            if (particle.FadeBehavior == FadeBehavior.FadeOut)
            {
                alpha = GameMath.Lerp(255, 0, progressStatus.endPercentage);
            }
            else
            {
                alpha = GameMath.Lerp(0, 255, progressStatus.endPercentage);
            }

            var currentColor = graphicsComponent.Color;
            graphicsComponent.Color = Color.FromArgb((byte)alpha, currentColor.R, currentColor.G, currentColor.B);
        }

        private struct ProgressStatus
        {
            public float startPercentage;
            public float endPercentage;
        }
    }
}
