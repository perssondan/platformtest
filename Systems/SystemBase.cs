﻿using System;
using uwpKarate.GameObjects;

namespace uwpKarate.Systems
{
    public abstract class SystemBase<T> : ISystem<T>
        where T : new()
    {
        protected SystemBase()
        {
            Initialize();
        }

        public string Name => typeof(T).Name;

        public static T Instance { get; } = new T();

        public abstract void Update(World world, TimeSpan deltaTime);

        protected virtual void Initialize()
        {
        }
    }
}