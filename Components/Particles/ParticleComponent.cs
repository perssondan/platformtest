﻿using System;
using uwpPlatformer.GameObjects;
using Windows.UI;

namespace uwpPlatformer.Components.Particles
{
    public class ParticleComponent : ComponentBase
    {
        public ParticleComponent(GameObject gameObject,
                                 TimeSpan timeToLive,
                                 Color startColor,
                                 Color endColor,
                                 TransitionBehavior changeColorBehavior,
                                 TimeSpan createdAt)
            : base(gameObject)
        {
            StartColor = startColor;
            EndColor = endColor;
            TransitionBehavior = changeColorBehavior;
            TimeToLive = timeToLive;
            CreatedAt = createdAt;
        }

        public TimeSpan TimeToLive { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public TransitionBehavior TransitionBehavior { get; set; }
        public FadeBehavior FadeBehavior { get; set; }
        public ParticleSizeBehavior ParticleSizeBehavior { get; set; }
        public TimeSpan CreatedAt { get; }
        public TimeSpan EndOfLife => CreatedAt + TimeToLive;
    }
}
