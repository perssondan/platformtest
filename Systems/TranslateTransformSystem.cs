using GamesLibrary.Models;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class TranslateTransformSystem : ISystem
    {
        private readonly IGameObjectManager _gameObjectManager;

        public TranslateTransformSystem(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(TranslateTransformSystem);

        public void Update(TimingInfo timingInfo)
        {
            float deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;

            _gameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponents<TransformComponent, PhysicsComponent>())
                .Where(components => components != default)
                .ToArray()
                .ForEach(components => Translate(components.Item1, components.Item2));
        }

        private void Translate(TransformComponent transformComponent, PhysicsComponent physicsComponent)
        {
            transformComponent.Position = physicsComponent.Position;
        }
    }
}
