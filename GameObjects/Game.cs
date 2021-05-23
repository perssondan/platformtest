using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using uwpPlatformer.Systems;

namespace uwpPlatformer.GameObjects
{
    public class Game
    {
        private readonly ColliderSystem _colliderSystem;
        private readonly MoveSystem _moveSystem;
        private readonly PhysicsSystem _physicsSystem;
        private readonly InputSystem _inputSystem;
        private readonly GraphicsSystem _graphicsSystem;
        private readonly ParticleSystem _particleSystem;
        private readonly PlayerSystem _playerSystem;
        private readonly IEventSystem _eventSystem = new EventSystem();

        public Game(Windows.UI.Xaml.Window current)
        {
            _colliderSystem = new ColliderSystem(_eventSystem);
            _moveSystem = new MoveSystem();
            _physicsSystem = new PhysicsSystem(_eventSystem);
            _inputSystem = new InputSystem(_eventSystem);
            _graphicsSystem = new GraphicsSystem(_eventSystem);
            _particleSystem = new ParticleSystem();
            _playerSystem = new PlayerSystem();

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
