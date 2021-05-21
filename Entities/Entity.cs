using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uwpKarate.Components;

namespace uwpKarate.Entities
{
    public struct Entity : IEquatable<Entity>, IFormattable
    {
        public static int _idCounter = 1;
        private int? _id;

        public static readonly Entity Zero = new Entity(0);

        public int Id => InitializeId();

        public Entity(int id)
        {
            _id = id;
        }

        private int InitializeId()
        {
            if (!_id.HasValue)
            {
                _id = _idCounter++;
            }
            return _id.Value;
        }

        public static implicit operator Entity(int id)
        {
            return new Entity(id);
        }

        public bool Equals(Entity other)
        {
            return Id == other.Id;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"Id={Id}";
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity entity)) return false;

            return Equals(entity);
        }

        public override string ToString()
        {
            return ToString(string.Empty, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }

    public static class Entities
    {
        private static readonly IDictionary<Entity, IDictionary<Type, IComponent>> _entities = new Dictionary<Entity, IDictionary<Type, IComponent>>();

        public static void Add(Entity entity)
        {
            _entities.TryAdd(entity, new Dictionary<Type, IComponent>());
        }

        public static void Remove(Entity entity)
        {
            _entities.Remove(entity);
        }

        public static void Remove(int id)
        {
            Entity entity = id;
            _entities.Remove(entity);
        }

        public static Entity GetEntity(int id)
        {
            // haha this looks silly
            Entity entity = id; new Entity();
            return entity;
        }
    }
}
