using System;
using System.Collections.Generic;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.GameObjects;
using Windows.UI;
using uwpPlatformer.Extensions;

namespace uwpPlatformer.Systems
{
    public class ParticleSystem : SystemBase<ParticleSystem>
    {
        public override void Update(TimingInfo timingInfo)
        {
            // Make a copy of the list before thus not to change the list we're iterating
            var particles = ParticleComponentManager.Instance.Components.ToArray();

            foreach (var particle in particles)
            {
                UpdateParticleColor(particle);
                particle.TimeToLive -= deltaTime;
                if (particle.TimeToLive > TimeSpan.Zero)
                {
                    continue;
                }

                particle.GameObject.Dispose();
            }
        }

        private void UpdateParticleColor(ParticleComponent particle)
        {
            var graphicsComponent = particle.GameObject.GetComponent<ShapeGraphicsComponent>();
            if (graphicsComponent is null) return;

            var startColorPercentage = (particle.TimeToLive.TotalMilliseconds / particle.InitialTimeToLive.TotalMilliseconds);
            var endColorPercentage = 1f - startColorPercentage;
            graphicsComponent.Color = GetNextColor(particle.StartColor, particle.EndColor, (float)endColorPercentage);
        }

        private Color GetNextColor(Color startColor, Color endColor, float percentage)
        {
            var alpha = Lerp(startColor.A, endColor.A, percentage);
            var red = Lerp(startColor.R, endColor.R, percentage);
            var green = Lerp(startColor.G, endColor.G, percentage);
            var blue = Lerp(startColor.B, endColor.B, percentage);

            return Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
        }

        private float Lerp(float startValue, float endValue, float lerpValue)
        {
            return startValue * (1 - lerpValue) + endValue * lerpValue;
        }
    }
}
