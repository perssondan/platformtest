using GamesLibrary.Entities;

namespace GamesLibrary.Models
{
    public struct EntityAdded
    {
        public EntityAdded(Entity entity)
        {
            Entity = entity;
        }

        public readonly Entity Entity;
    }
}
