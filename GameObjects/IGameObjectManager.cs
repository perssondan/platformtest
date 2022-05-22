using System.Collections.Generic;

namespace uwpPlatformer.GameObjects
{
    public interface IGameObjectManager
    {
        GameObject CreateGameObject();

        void RemoveGameObject(GameObject gameObject);

        void DestroyGameObject(GameObject gameObject);

        void Update();

        IReadOnlyList<GameObject> GameObjects { get; }
    }
}
