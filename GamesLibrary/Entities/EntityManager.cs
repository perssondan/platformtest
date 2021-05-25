using GamesLibrary.Components;
using GamesLibrary.Models;
using GamesLibrary.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GamesLibrary.Entities
{
    public class EntityManager : IEntityManager
    {
        private readonly IDictionary<Entity, IDictionary<Type, IComponent>> _entities = new Dictionary<Entity, IDictionary<Type, IComponent>>();
        private readonly IDictionary<string, IList<Entity>> _groupedEntities = new Dictionary<string, IList<Entity>>();
        private readonly IEventSystem _eventSystem;

        public EntityManager(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
        }

        public Entity CreateEntity() => CreateEntity(string.Empty);

        public Entity CreateEntity(string tag)
        {
            var entity = Entity.CreateEntity();

            _entities.Add(entity, new Dictionary<Type, IComponent>());
            AddEntityToGroupedEntities(entity, tag);

            _eventSystem?.Send(this, new EntityAdded(entity));

            return entity;
        }

        public void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent
        {
            if (!_entities.ContainsKey(entity))
                throw new InvalidOperationException("The entity does not exist");

            if (_entities[entity].ContainsKey(typeof(TComponent))) return;

            _entities[entity].Add(typeof(TComponent), component);

            _eventSystem?.Send(this, new ComponentAdded(component));
        }

        public void Remove(Entity entity)
        {
            if (!_entities.Remove(entity)) return;

            RemoveEntityFromGroupedEntities(entity);

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

        public IReadOnlyList<Entity> GetEntities(string tag)
        {
            tag = tag ?? string.Empty;

            if (_groupedEntities.TryGetValue(tag, out var entities))
            {
                return entities.ToArray();
            }
            return Array.Empty<Entity>();
            
        }

        public TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent
        {
            if (!_entities.TryGetValue(entity, out var components)) return default(TComponent);

            if (!components.TryGetValue(typeof(TComponent), out var component)) return default(TComponent);

            return (TComponent)component;
        }

        public (TComponentFirst, TComponentSecond) GetComponents<TComponentFirst, TComponentSecond>(Entity entity)
            where TComponentFirst : IComponent
            where TComponentSecond : IComponent
        {
            var firstComponent = GetComponent<TComponentFirst>(entity);
            var secondComponent = GetComponent<TComponentSecond>(entity);

            if (firstComponent == null || secondComponent == null)
                return default((TComponentFirst, TComponentSecond));

            return (firstComponent, secondComponent);
        }

        private void AddEntityToGroupedEntities(Entity entity, string tag)
        {
            tag = tag ?? string.Empty;
            if (!_groupedEntities.ContainsKey(tag))
            {
                _groupedEntities.Add(tag, new List<Entity>());
            }

            _groupedEntities[tag].Add(entity);
        }

        private void RemoveEntityFromGroupedEntities(Entity entity)
        {
            foreach (var entities in _groupedEntities.Values)
            {
                entities.Remove(entity);
            }
        }
    }
}
