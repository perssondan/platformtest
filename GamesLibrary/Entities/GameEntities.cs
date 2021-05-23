using GamesLibrary.Components;
using GamesLibrary.Models;
using GamesLibrary.Systems;
using System;
using System.Collections.Generic;

namespace GamesLibrary.Entities
{
    public class GameEntities : IEntities
    {
        private readonly IDictionary<Entity, IDictionary<Type, IComponent>> _entities = new Dictionary<Entity, IDictionary<Type, IComponent>>();
        private readonly IEventSystem _eventSystem;

        public GameEntities(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public void Add(Entity entity)
        {
            if (_entities.ContainsKey(entity)) return;

            _entities.Add(entity, new Dictionary<Type, IComponent>());

            _eventSystem?.Send(this, new EntityAdded(entity));
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            Add(entity);

            if (_entities[entity].ContainsKey(typeof(TComponent))) return;

            _entities[entity].Add(typeof(TComponent), component);

            _eventSystem?.Send(this, new ComponentAdded(component));
        }

        public void Remove(Entity entity)
        {
            if (!_entities.Remove(entity)) return;

            _eventSystem?.Send(this, new EntityRemoved(entity));
        }

        public void Remove(int id)
        {
            Entity entity = id;
            Remove(entity);
        }

        public void RemoveComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (!_entities.TryGetValue(entity, out var components)) return;

            if (!components.Remove(typeof(TComponent))) return;

            _eventSystem?.Send(this, new ComponentRemoved(component));
        }

        public Entity GetEntity(int id)
        {
            Entity entity = id;

            if (!_entities.ContainsKey(entity)) return Entity.Zero;

            return entity;
        }
    }
}
