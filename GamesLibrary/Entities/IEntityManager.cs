using GamesLibrary.Components;
using System.Collections.Generic;

namespace GamesLibrary.Entities
{
    public interface IEntityManager
    {
        Entity CreateEntity();

        Entity CreateEntity(string tag);

        void AddComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent;

        Entity GetEntity(int id);

        IReadOnlyList<Entity> GetEntities(string tag);

        TComponent GetComponent<TComponent>(Entity entity)
            where TComponent : IComponent;

        (TComponentFirst, TComponentSecond) GetComponents<TComponentFirst, TComponentSecond>(Entity entity)
            where TComponentFirst : IComponent
            where TComponentSecond : IComponent;

        void Remove(Entity entity);

        void Remove(int id);

        void RemoveComponent<TComponent>(Entity entity, TComponent component)
            where TComponent : IComponent;
    }
}
