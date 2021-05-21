﻿using System;
using System.Collections.Generic;
using uwpPlatformer.Components;

namespace uwpPlatformer.GameObjects
{
    public class GameObject : IDisposable
    {
        public static int _idCounter = 0;
        private readonly IDictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        public GameObject()
        {
            Id = GenerateId();
            GameObjectManager.AddGameObject(this);
            AddComponent(new TransformComponent(this));
        }

        public int Id { get; }

        public void AddComponent<T>(T gameObjectComponent)
            where T : IComponent
        {
            if (gameObjectComponent == null) return;

            _components[typeof(T)] = gameObjectComponent;
        }

        public T GetComponent<T>()
            where T : IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }

            return default;
        }

        public (T, U) GetComponent<T, U>()
            where T : IComponent
            where U : IComponent
        {
            if (_components.TryGetValue(typeof(T), out var componentT) && _components.TryGetValue(typeof(U), out var componentU))
            {
                return ((T)componentT, (U)componentU);
            }

            return default;
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

        private static int GenerateId()
        {
            return _idCounter++;
        }
    }
}