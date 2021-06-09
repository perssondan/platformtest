using System.Collections.Generic;

namespace uwpPlatformer.GameObjects
{
    public class GameObjectManager : IGameObjectManager
    {
        private List<GameObject> _gameObjects = new List<GameObject>();

        public GameObject CreateGameObject()
        {
            var gameObject = new GameObject();
            _gameObjects.Add(gameObject);
            return gameObject;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            // TODO: remove components
            _gameObjects.Remove(gameObject);
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            _gameObjects.Remove(gameObject);
            gameObject.Dispose();
        }

        public IReadOnlyList<GameObject> GameObjects => _gameObjects.AsReadOnly();
    }
}