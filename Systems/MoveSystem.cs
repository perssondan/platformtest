using GamesLibrary.Models;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class MoveSystem : SystemBase<MoveSystem>
    {
        private readonly IGameObjectManager _gameObjectManager;

        public MoveSystem(IGameObjectManager gameObjectManager)
        {
            _gameObjectManager = gameObjectManager;
        }

        public override void Update(TimingInfo timingInfo)
        {
            float deltaTime = (float)timingInfo.ElapsedTime.TotalSeconds;

            _gameObjectManager.GameObjects
                .Select(gameObject => gameObject.GetComponent<TransformComponent>())
                .Where(transformComponent => transformComponent != default)
                .ToArray()
                .ForEach(transformComponent => Translate(transformComponent, deltaTime));
        }

        private void Translate(TransformComponent transformComponent, float deltaTime)
        {
            if (transformComponent.Velocity.LengthSquared() == 0f) return;

            if (transformComponent.Velocity.Y < 0.5f && transformComponent.Velocity.Y > -0.5f)
            {
                transformComponent.Velocity *= Vector2.UnitX;
            }

            if (transformComponent.Velocity.X < 0.5f && transformComponent.Velocity.X > -0.5f)
            {
                transformComponent.Velocity *= Vector2.UnitY;
            }

            var position = transformComponent.Velocity * deltaTime;
            if (transformComponent.Position == position) return;

            transformComponent.Position += position;
        }
    }
}
