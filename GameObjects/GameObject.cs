using System;
using System.Collections.Generic;
using uwpKarate.Components;

namespace uwpKarate.GameObjects
{
    public class GameObject : IDisposable
    {
        private readonly IDictionary<Type, IGameObjectComponent> _components = new Dictionary<Type, IGameObjectComponent>();

        public GameObject()
        {
            GameObjectManager.AddGameObject(this);
            AddComponent(new TransformComponent(this));
        }

        public void AddComponent<T>(T gameObjectComponent)
            where T : IGameObjectComponent
        {
            if (gameObjectComponent == null) return;

            _components[typeof(T)] = gameObjectComponent;
        }

        public T GetComponent<T>()
            where T : IGameObjectComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }

            return default;
        }

        public void Update(World world, TimeSpan timeSpan)
        {
            OnBeforeUpdate(world, timeSpan);

            OnAfterUpdate(world, timeSpan);
        }

        public virtual void OnBeforeUpdate(World world, TimeSpan timeSpan)
        {
        }

        public virtual void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
        }

        public void Dispose()
        {
            GameObjectManager.RemoveGameObject(this);
            foreach (var component in _components.Values)
            {
                component.Dispose();
            }
        }

        public AnimatedGraphicsComponent GraphicsComponent => GetComponent<AnimatedGraphicsComponent>();
        public PhysicsComponent PhysicsComponent => GetComponent<PhysicsComponent>();
        public InputComponent InputComponent => GetComponent<InputComponent>();
        public TransformComponent TransformComponent => GetComponent<TransformComponent>();
        public ColliderComponent ColliderComponent => GetComponent<ColliderComponent>();
    }
}