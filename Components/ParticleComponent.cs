using System;
using uwpKarate.GameObjects;
using Windows.UI;

namespace uwpKarate.Components
{
    public class ParticleComponent : GameObjectComponent
    {
        public ParticleComponent(GameObject gameObject,
                                 TimeSpan initialTimeToLive,
                                 Color startColor,
                                 Color endColor,
                                 ChangeColorBehavior changeColorBehavior)
            : base(gameObject)
        {
            InitialTimeToLive = initialTimeToLive;
            StartColor = startColor;
            EndColor = endColor;
            ChangeColorBehavior = changeColorBehavior;
            TimeToLive = initialTimeToLive;
            ParticleComponentManager.Instance.AddComponent(this);
        }

        public TimeSpan TimeToLive { get; set; }
        public TimeSpan InitialTimeToLive { get; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public ChangeColorBehavior ChangeColorBehavior { get; set; }

        protected override void OnDispose()
        {
            ParticleComponentManager.Instance.RemoveComponent(this);
        }
    }

    public enum ChangeColorBehavior
    {
        OverTime,
        OverLength,
        OverVelocity
    }
}
