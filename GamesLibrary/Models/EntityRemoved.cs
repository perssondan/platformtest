using GamesLibrary.Entities;

namespace GamesLibrary.Models
{
    public struct EntityRemoved
    {
        public EntityRemoved(Entity entity)
        {
            Entity = entity;
        }

        public readonly Entity Entity;
    }
}
