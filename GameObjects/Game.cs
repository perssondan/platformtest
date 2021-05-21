using Microsoft.Graphics.Canvas;
using System;
using uwpPlatformer.Systems;

namespace uwpPlatformer.GameObjects
{
    public class Game
    {
        private ColliderSystem _colliderSystem = ColliderSystem.Instance;
        private MoveSystem _moveSystem = MoveSystem.Instance;
        private PhysicsSystem _physicsSystem = PhysicsSystem.Instance;
        private InputSystem _inputSystem = InputSystem.Instance;
        private GraphicsSystem _graphicsSystem = GraphicsSystem.Instance;
        private ParticleSystem _particleSystem = ParticleSystem.Instance;
        private PlayerSystem _playerSystem = PlayerSystem.Instance;

        public Game(Windows.UI.Xaml.Window current)
        {
            _inputSystem.Current = current;
        }

        public void Update(TimeSpan deltaTime)
        {
            _inputSystem.Update(deltaTime);
            _graphicsSystem.Update(deltaTime);
            _playerSystem.Update(deltaTime);
            _particleSystem.Update(deltaTime);
            _physicsSystem.Update(deltaTime);
            _colliderSystem.Update(deltaTime);

            // If we still have collisions, resolve them now!
            _colliderSystem.ResolveCollisions(deltaTime);
            _moveSystem.Update(deltaTime);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
        }
    }
}
