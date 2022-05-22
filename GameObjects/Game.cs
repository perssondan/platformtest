using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly TranslateTransformSystem _transformSystem;
        private readonly PhysicsSystem _physicsSystem;
        private readonly InputSystem _inputSystem;
        private readonly GraphicsSystem _graphicsSystem;
        private readonly ParticleSystem _particleSystem;
        private readonly PlayerSystem _playerSystem;
        private readonly DustParticleEmitterSystem _dustParticleEmitterSystem;
        private readonly ParticleEmitterSystem _particleEmitterSystem;
        private readonly IEventSystem _eventSystem = new EventSystem();
        private readonly DustEntityFactory _dustEntityFactory;
        private readonly PerlinMoveSystem _perlinSystem;
        private readonly DebugSystem _debugSystem;
        private readonly World _world;

        public Game(Windows.UI.Xaml.Window current, CanvasBitmap[] canvasBitmaps, Map map, TileAtlas[] tileAtlases)
        {
            _colliderSystem = new ColliderSystem(_eventSystem, _gameObjectManager);
            _transformSystem = new TranslateTransformSystem(_gameObjectManager);
            _perlinSystem = new PerlinMoveSystem(_gameObjectManager);
            _physicsSystem = new PhysicsSystem(_gameObjectManager, _eventSystem);
            _inputSystem = new InputSystem(_eventSystem, _gameObjectManager);
            _graphicsSystem = new GraphicsSystem(_gameObjectManager);
            _particleSystem = new ParticleSystem(_gameObjectManager);
            _world = new World(canvasBitmaps, map, tileAtlases, _gameObjectManager, _eventSystem);
            _dustEntityFactory = new DustEntityFactory(_gameObjectManager);
            _playerSystem = new PlayerSystem(_eventSystem, _gameObjectManager);
            _dustParticleEmitterSystem = new DustParticleEmitterSystem(_eventSystem, _dustEntityFactory, _gameObjectManager);
            _particleEmitterSystem = new ParticleEmitterSystem(_dustEntityFactory, _gameObjectManager);
            _debugSystem = new DebugSystem(_eventSystem, _gameObjectManager);

            _inputSystem.Current = current;

            _eventSystem.Subscribe<CollisionEvent>(this, (sender, collisionEvent) =>
            {
                _hasCollisions = true;

                var isHero = collisionEvent.GameObject.Has<HeroComponent>();
                var isEnemy = collisionEvent.IsCollidingWith.Has<EnemyComponent>();

                if (isHero && isEnemy)
                {
                    Debug.WriteLine("Hero collided with enemy!");
                }
            });
        }

        public World World => _world;

        public void Update(TimingInfo timingInfo)
        {
            _hasCollisions = false;

            _gameObjectManager.Update();

            _inputSystem.Update(timingInfo);
            _perlinSystem.Update(timingInfo);
            _graphicsSystem.Update(timingInfo);
            _playerSystem.Update(timingInfo);
            _particleSystem.Update(timingInfo);

            _physicsSystem.Update(timingInfo);
            _colliderSystem.Update(timingInfo);

            _dustParticleEmitterSystem.Update(timingInfo);
            _particleEmitterSystem.Update(timingInfo);

            ResolveCollisions(timingInfo);

            _transformSystem.Update(timingInfo);

            _world.Update(timingInfo);
        }

        private void ResolveCollisions(TimingInfo timingInfo)
        {
            var maxResolveAttempts = 5;
            for (var currentResolveAttempt = 0; currentResolveAttempt < maxResolveAttempts && _hasCollisions; currentResolveAttempt++)
            {
                _hasCollisions = false;
                _physicsSystem.PostUpdate(timingInfo);
                _colliderSystem.Update(timingInfo);
            }
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
            _debugSystem.Draw(canvasDrawingSession, timeSpan);
        }
    }
}
