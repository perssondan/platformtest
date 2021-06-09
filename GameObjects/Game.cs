using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.Events;
using uwpPlatformer.Factories;
using uwpPlatformer.Models;
using uwpPlatformer.Systems;

namespace uwpPlatformer.GameObjects
{
    public class Game
    {
        private IDictionary<string, IScene> _scenes = new Dictionary<string, IScene>();

        private IGameObjectManager _gameObjectManager = new GameObjectManager();
        private bool _hasCollisions = false;
        // TODO: DI time...
        private readonly ColliderSystem _colliderSystem;
        private readonly MoveSystem _moveSystem;
        private readonly PhysicsSystem _physicsSystem;
        private readonly InputSystem _inputSystem;
        private readonly GraphicsSystem _graphicsSystem;
        private readonly ParticleSystem _particleSystem;
        private readonly PlayerSystem _playerSystem;
        private readonly DustParticleEmitterSystem _dustParticleEmitterSystem;
        private readonly ParticleEmitterSystem _particleEmitterSystem;
        private readonly IEventSystem _eventSystem = new EventSystem();
        private readonly PerlinSystem _perlinSystem = new PerlinSystem();
        private readonly DustEntityFactory _dustEntityFactory;
        private readonly World _world;

        public Game(Windows.UI.Xaml.Window current, CanvasBitmap[] canvasBitmaps, Map map, TileAtlas[] tileAtlases)
        {
            _colliderSystem = new ColliderSystem(_eventSystem);
            _moveSystem = new MoveSystem(_gameObjectManager);
            _physicsSystem = new PhysicsSystem(_gameObjectManager);
            _inputSystem = new InputSystem(_eventSystem, _gameObjectManager);
            _graphicsSystem = new GraphicsSystem(_eventSystem, _gameObjectManager);
            _particleSystem = new ParticleSystem(_gameObjectManager);
            _world = new World(canvasBitmaps, map, tileAtlases, _gameObjectManager);
            _dustEntityFactory = new DustEntityFactory(_gameObjectManager);
            _playerSystem = new PlayerSystem(_eventSystem);
            _dustParticleEmitterSystem = new DustParticleEmitterSystem(_eventSystem, _dustEntityFactory, _gameObjectManager);
            _particleEmitterSystem = new ParticleEmitterSystem(_dustEntityFactory, _gameObjectManager);

            _inputSystem.Current = current;

            _eventSystem.Subscribe<CollisionEvent>(this, (sender, collisionEvent) =>
            {
                _hasCollisions = true;
            });
        }

        public World World => _world;
        public void Update(TimingInfo timingInfo)
        {
            _hasCollisions = false;

            _inputSystem.Update(timingInfo);
            _perlinSystem.Update(timingInfo);
            _graphicsSystem.Update(timingInfo);
            _playerSystem.Update(timingInfo);
            _particleSystem.Update(timingInfo);

            _physicsSystem.Update(timingInfo);
            _colliderSystem.Update(timingInfo);
            _dustParticleEmitterSystem.Update(timingInfo);
            _particleEmitterSystem.Update(timingInfo);

            // If we have collisions, resolve them now!
            if (_hasCollisions)
            {
                _colliderSystem.ResolveCollisions(timingInfo);
            }

            _moveSystem.Update(timingInfo);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
        }
    }
}
