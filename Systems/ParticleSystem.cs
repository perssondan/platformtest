using GamesLibrary.Models;
using System;
using System.Collections;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.Components.Particles;
using uwpPlatformer.Extensions;

namespace uwpPlatformer.Systems
{
    public class ParticleSystem : SystemBase<ParticleSystem>
    {
        public ParticleSystem()
        {
        }

        public override void Update(TimingInfo timingInfo)
        {
            // Make a copy of the list before thus not to change the list we're iterating
            var particles = ParticleComponentManager.Instance.Components.ToArray();

            var now = timingInfo.TotalTime;
            foreach (var particle in particles)
            {
                var elapsedTime = now - particle.CreatedAt;

                if (elapsedTime < particle.TimeToLive)
                {
                    UpdateParticleColor(particle, elapsedTime);
                    continue;
                }

                particle.GameObject.Dispose();
                //_entityManager.Remove(entity);
            }
        }

        private void UpdateParticleColor(ParticleComponent particle, TimeSpan elapsedTime)
        {
            var graphicsComponent = particle.GameObject.GetComponent<ShapeGraphicsComponent>();
            if (graphicsComponent is null) return;

            var ticksDifference = Math.Clamp(particle.TimeToLive.Ticks - elapsedTime.Ticks, 0f, particle.TimeToLive.Ticks);
            var startColorPercentage = ticksDifference / particle.TimeToLive.Ticks;
            var endColorPercentage = 1f - startColorPercentage;
            var nextColor = particle.StartColor.Lerp(particle.EndColor, endColorPercentage);
            graphicsComponent.Color = nextColor;
        }
    }
}
