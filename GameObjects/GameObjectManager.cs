using System;
using System.Collections.Generic;

namespace uwpPlatformer.GameObjects
{
    public class GameObjectManager : IGameObjectManager
    {
        private List<GameObject> _activeGameObjects = new List<GameObject>();
        private List<GameObject> _destroyedGameObjects = new List<GameObject>();
        private List<GameObject> _removedGameObjects = new List<GameObject>();
        private List<GameObject> _addedGameObjects = new List<GameObject>();

        public GameObject CreateGameObject()
        {
            var gameObject = new GameObject();
            _addedGameObjects.Add(gameObject);
            return gameObject;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            _removedGameObjects.Add(gameObject);
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            _destroyedGameObjects.Add(gameObject);
        }

        public void Update()
        {
            _activeGameObjects.AddRange(_addedGameObjects);
            _removedGameObjects.ForEach(go => _activeGameObjects.Remove(go));
            _destroyedGameObjects.ForEach(go => _activeGameObjects.Remove(go));

            _destroyedGameObjects.ForEach(go => go.Dispose());

            _addedGameObjects.Clear();
            _removedGameObjects.Clear();
            _destroyedGameObjects.Clear();

            GameObjects = _activeGameObjects.AsReadOnly();
        }

        public IReadOnlyList<GameObject> GameObjects { get; private set; } = Array.Empty<GameObject>();
    }
}
