﻿using System;
using uwpPlatformer.GameObjects;
using Windows.UI;

namespace uwpPlatformer.Components
{
    public class ParticleComponent : GameObjectComponent
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
            ParticleComponentManager.Instance.AddComponent(this);
        }

        public TimeSpan TimeToLive { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public TransitionBehavior TransitionBehavior { get; set; }
        public FadeBehavior FadeBehavior { get; set; }
        public ParticleSizeBehavior ParticleSizeBehavior { get; set; }
        public TimeSpan CreatedAt { get; }
        public TimeSpan EndOfLife => CreatedAt + TimeToLive;

        protected override void OnDispose()
        {
            ParticleComponentManager.Instance.RemoveComponent(this);
        }
    }

    // 1 bit
    public enum ColorBehavior
    {
        None = 0,
        Lerp = 1,
    }

    // 3 bit
    public enum TransitionBehavior
    {
        None = 0,
        OverTime = 1,
        OverLength = 2,
        OverVelocity = 3
    }

    // 2 bit
    public enum FadeBehavior
    {
        None = 0,
        FadeIn = 1,
        FadeOut = 2
    }

    // 2 bit
    public enum ParticleSizeBehavior
    {
        None = 0,
        Grow = 1,
        Shrink = 2
    }
}
