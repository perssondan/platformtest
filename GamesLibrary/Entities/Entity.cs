using System;
using System.Runtime.CompilerServices;

namespace GamesLibrary.Entities
{
    public struct Entity : IEquatable<Entity>, IFormattable
    {
        private static int _idCounter = 1;

        public static readonly Entity Zero = new Entity(0);

        public static Entity CreateEntity() => new Entity(_idCounter++);

        public readonly int Id;

        public Entity(int id)
        {
            Id = id;
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
}
