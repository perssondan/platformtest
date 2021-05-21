using System.Collections.Generic;

namespace uwpPlatformer.Components
{
    public abstract class ComponentManager<T>
    {
        private static List<T> _components = new List<T>();

        public void AddComponent(T component)
        {
            _components.Add(component);
        }

        public void RemoveComponent(T component)
        {
            _components.Remove(component);
        }

        public IReadOnlyList<T> Components => _components.AsReadOnly();
    }
}