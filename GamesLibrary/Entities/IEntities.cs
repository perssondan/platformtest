using GamesLibrary.Components;

namespace GamesLibrary.Entities
{
    public interface IEntities
    {
        void Add(Entity entity);

        void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent;

        Entity GetEntity(int id);

        void Remove(Entity entity);

        void Remove(int id);

        void RemoveComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent;
    }
}
