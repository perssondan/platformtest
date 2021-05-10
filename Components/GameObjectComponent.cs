﻿using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public abstract class GameObjectComponent : IGameObjectComponent
    {
        public GameObjectComponent(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public GameObject GameObject { get; set; }
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            OnDispose();
            GameObject = null;
            IsDisposed = true;
        }

        protected virtual void OnDispose()
        {

        }
    }
}
