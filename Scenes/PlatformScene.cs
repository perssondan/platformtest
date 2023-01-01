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
using GamesLibrary.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Platform;
using Windows.System;
using System.Numerics;
using uwpPlatformer.Numerics;

namespace uwpPlatformer.Scenes
{
    public class PlatformScene : Scene
    {
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
        private readonly IGameAssetsProvider _gameAssetsProvider;

        public PlatformScene(IGameAssetsProvider gameAssetsProvider)
        {
            _gameAssetsProvider = gameAssetsProvider;

            _colliderSystem = new ColliderSystem(_eventSystem, _gameObjectManager);
            _transformSystem = new TranslateTransformSystem(_gameObjectManager);
            _perlinSystem = new PerlinMoveSystem(_gameObjectManager);
            _physicsSystem = new PhysicsSystem(_gameObjectManager, _eventSystem);
            _inputSystem = new InputSystem(_eventSystem, _gameObjectManager);
            _graphicsSystem = new GraphicsSystem(_gameObjectManager);
            _particleSystem = new ParticleSystem(_gameObjectManager);
            _world = new World(_gameAssetsProvider.Bitmaps, _gameAssetsProvider.Map, _gameAssetsProvider.TileAtlases, _gameObjectManager, _eventSystem);
            _dustEntityFactory = new DustEntityFactory(_gameObjectManager);
            _playerSystem = new PlayerSystem(_eventSystem, _gameObjectManager);
            _dustParticleEmitterSystem = new DustParticleEmitterSystem(_eventSystem, _dustEntityFactory, _gameObjectManager);
            _particleEmitterSystem = new ParticleEmitterSystem(_dustEntityFactory, _gameObjectManager);
            _debugSystem = new DebugSystem(_eventSystem, _gameObjectManager);

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

        public override string Name => typeof(PlatformScene).Name;

        public override void Draw(CanvasDrawingSession canvasDrawingSession, TimeSpan timeSpan)
        {
            _graphicsSystem.Draw(canvasDrawingSession, timeSpan);
            _debugSystem.Draw(canvasDrawingSession, timeSpan);
        }

        public override void Update(TimingInfo timingInfo)
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

        public override void Init()
        {
            _colliderSystem.Init();
            _transformSystem.Init();
            _perlinSystem.Init();
            _physicsSystem.Init();
            _inputSystem.Init();
            _graphicsSystem.Init();
            _particleSystem.Init();
            _playerSystem.Init();
            _dustParticleEmitterSystem.Init();
            _particleEmitterSystem.Init();
            _debugSystem.Init();
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
    }
}
