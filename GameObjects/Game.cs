using GamesLibrary.Models;
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
        private readonly DustParticleEmitterSystem _dustParticleEmitterSystem;
        private readonly IEventSystem _eventSystem = new EventSystem();

        public Game(Windows.UI.Xaml.Window current)
        {
            _colliderSystem = new ColliderSystem(_eventSystem);
            _moveSystem = new MoveSystem();
            _physicsSystem = new PhysicsSystem(_eventSystem);
            _inputSystem = new InputSystem(_eventSystem);
            _graphicsSystem = new GraphicsSystem(_eventSystem);
            _particleSystem = new ParticleSystem();
            _playerSystem = new PlayerSystem(_eventSystem);
            _dustParticleEmitterSystem = new DustParticleEmitterSystem(_eventSystem);


            _inputSystem.Current = current;
        }

        public void Update(TimingInfo timingInfo)
        {
            _inputSystem.Update(timingInfo);
            _graphicsSystem.Update(timingInfo);
            _playerSystem.Update(timingInfo);
            _particleSystem.Update(timingInfo);
            _physicsSystem.Update(timingInfo);
            _colliderSystem.Update(timingInfo);
            _dustParticleEmitterSystem.Update(timingInfo);

            // If we still have collisions, resolve them now!
            _colliderSystem.ResolveCollisions(timingInfo);
            _moveSystem.Update(timingInfo);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
        }
    }
}
