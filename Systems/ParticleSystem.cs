using System;
using System.Linq;
using uwpKarate.Components;
using uwpKarate.GameObjects;

namespace uwpKarate.Systems
{
    public class ParticleSystem : SystemBase<ParticleSystem>
    {
        public override void Update(World world, TimeSpan deltaTime)
        {
            var particles = ParticleComponentManager.Instance.Components.ToArray();

            foreach (var particle in particles)
            {
                particle.EllapsedTime += deltaTime;
                if (particle.EllapsedTime > particle.TimeToLive)
                {
                    particle.GameObject.Dispose();
                }
            }
        }
    }
}
