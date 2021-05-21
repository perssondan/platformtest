using System.Collections.Generic;

namespace uwpPlatformer.GameObjects
{
    public static class GameObjectManager
    {
        private static List<GameObject> _gameObjects = new List<GameObject>();

        public static void AddGameObject(GameObject gameObject)
        {
            _gameObjects.Add(gameObject);
        }

        public static void RemoveGameObject(GameObject gameObject)
        {
            // TODO: remove components
            _gameObjects.Remove(gameObject);
        }

        public static IReadOnlyList<GameObject> GameObjects => _gameObjects.AsReadOnly();
    }
}