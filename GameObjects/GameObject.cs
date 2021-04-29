using System;
using System.Collections.Generic;
using uwpKarate.Components;

namespace uwpKarate.GameObjects
{
    public class GameObject
    {
        private IDictionary<Type, IGameObjectComponent> _components = new Dictionary<Type, IGameObjectComponent>();

        public GameObject()
        {
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

            InputComponent?.Update(world, timeSpan);
            PhysicsComponent?.Update(world, timeSpan);
            ColliderComponent?.Update(world, timeSpan);
            // Must be last
            TransformComponent?.Update(world, timeSpan);

            OnAfterUpdate(world, timeSpan);
        }

        public virtual void OnBeforeUpdate(World world, TimeSpan timeSpan)
        {
        }

        public virtual void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
        }

        public GraphicsComponent GraphicsComponent => GetComponent<GraphicsComponent>();
        public PhysicsComponent PhysicsComponent => GetComponent<PhysicsComponent>();
        public InputComponent InputComponent => GetComponent<InputComponent>();
        public TransformComponent TransformComponent => GetComponent<TransformComponent>();
        public ColliderComponent ColliderComponent => GetComponent<ColliderComponent>();
    }
}