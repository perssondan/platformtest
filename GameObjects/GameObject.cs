using System;
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
            AddOrUpdateComponent(new TransformComponent(this));
        }

        public int Id { get; }

        public void AddOrUpdateComponent<T>(T gameObjectComponent)
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

        public bool Has<T>()
            where T : IComponent
        {
            return _components.TryGetValue(typeof(T), out var _);
        }

        public bool Has<T, U>()
            where T : IComponent
            where U : IComponent
        {
            return _components.TryGetValue(typeof(T), out var _) &&
                   _components.TryGetValue(typeof(U), out var _);
        }

        public bool Has<T, U, V>()
            where T : IComponent
            where U : IComponent
            where V : IComponent
        {
            return _components.TryGetValue(typeof(T), out var _) &&
                   _components.TryGetValue(typeof(U), out var _) &&
                   _components.TryGetValue(typeof(V), out var _);
        }

        public void RemoveComponent<T>()
            where T : IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                _components.Remove(typeof(T));
                component.Dispose();
            }
        }

        public (T, U) GetComponents<T, U>()
            where T : IComponent
            where U : IComponent
        {
            if (_components.TryGetValue(typeof(T), out var componentT)
                && _components.TryGetValue(typeof(U), out var componentU))
            {
                return ((T)componentT, (U)componentU);
            }

            return default;
        }

        public (TComponent, UComponent, VComponent) GetComponents<TComponent, UComponent, VComponent>()
            where TComponent : IComponent
            where UComponent : IComponent
            where VComponent : IComponent
        {
            if (_components.TryGetValue(typeof(TComponent), out var componentT)
                && _components.TryGetValue(typeof(UComponent), out var componentU)
                && _components.TryGetValue(typeof(VComponent), out var componentV))
            {
                return ((TComponent)componentT, (UComponent)componentU, (VComponent)componentV);
            }

            return default;
        }

        public bool TryGetComponents<T, U>(out T componentT, out U componentU)
            where T : IComponent
            where U : IComponent
        {
            componentT = default;
            componentU = default;
            if (_components.TryGetValue(typeof(T), out var compT)
                && _components.TryGetValue(typeof(U), out var compU))
            {
                componentT = (T)compT;
                componentU = (U)compU;
                return true;
            }

            return default;
        }

        public bool TryGetComponent<T>(out T componentT)
            where T : IComponent
        {
            componentT = default;
            if (_components.TryGetValue(typeof(T), out var compT))
            {
                componentT = (T)compT;
                return true;
            }

            return default;
        }

        public void Dispose()
        {
            foreach (var component in _components.Values)
            {
                component.Dispose();
            }
        }

        private static int GenerateId()
        {
            return _idCounter++;
        }
    }
}
