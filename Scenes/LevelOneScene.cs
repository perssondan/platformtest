using GamesLibrary.Models;
using GamesLibrary.Systems;
using Microsoft.Graphics.Canvas;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Events;
using uwpPlatformer.Extensions;
using uwpPlatformer.Factories;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Models;
using uwpPlatformer.Platform;
using uwpPlatformer.Systems;

namespace uwpPlatformer.Scenes
{
    public class LevelOneScene : Scene
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
        private readonly HeroFactory _heroFactory;
        private readonly EnemyFactory _enemyFactory;

        public LevelOneScene(IGameAssetsProvider gameAssetsProvider)
        {
            _gameAssetsProvider = gameAssetsProvider;

            _colliderSystem = new ColliderSystem(_eventSystem, _gameObjectManager);
            _transformSystem = new TranslateTransformSystem(_gameObjectManager);
            _perlinSystem = new PerlinMoveSystem(_gameObjectManager);
            _physicsSystem = new PhysicsSystem(_gameObjectManager, _eventSystem);

            _inputSystem = new InputSystem(_eventSystem, _gameObjectManager);

            _graphicsSystem = new GraphicsSystem(_gameObjectManager);
            _particleSystem = new ParticleSystem(_gameObjectManager);
            _world = new World(_gameAssetsProvider, _gameObjectManager);
            _dustEntityFactory = new DustEntityFactory(_gameObjectManager);
            _playerSystem = new PlayerSystem(_eventSystem, _gameObjectManager);
            _dustParticleEmitterSystem = new DustParticleEmitterSystem(_eventSystem, _dustEntityFactory, _gameObjectManager);
            _particleEmitterSystem = new ParticleEmitterSystem(_dustEntityFactory, _gameObjectManager);
            _debugSystem = new DebugSystem(_eventSystem, _gameObjectManager);

            _heroFactory = new HeroFactory(_gameObjectManager);
            _enemyFactory = new EnemyFactory(_gameObjectManager);

            InitializeGameCharacters();
            InitializeHud();
        }

        public override string Name => nameof(LevelOneScene);

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

            //_world.Update(timingInfo);
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

        public override void Activate()
        {
            _eventSystem.Subscribe<CollisionEvent>(this, OnCollisionEvent);
        }

        public override void Deactivate()
        {
            _eventSystem.Unsubscribe<CollisionEvent>(this);
        }

        private void OnCollisionEvent(object sender, CollisionEvent collisionEvent)
        {
            _hasCollisions = true;

            var isHero = collisionEvent.GameObject.Has<HeroComponent>();
            var isEnemy = collisionEvent.IsCollidingWith.Has<EnemyComponent>();

            if (isHero && isEnemy)
            {
                Debug.WriteLine("Hero collided with enemy!");
            }
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

        private void InitializeGameCharacters()
        {
            var layerObjects = _gameAssetsProvider.Map.Layers
                .Where(x => x.LayerType.Equals("objectgroup", StringComparison.InvariantCultureIgnoreCase) && x.Name.Equals("characterLayer", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(x => x.LayerObjects)
                .ToArray();

            foreach (var layerObject in layerObjects)
            {
                AddGameCharacterEntities(layerObject);
            }
        }

        private void InitializeHud()
        {
            // Get all layerObjects from the hudOverlayLayer that we should render
            var layerObjects = _gameAssetsProvider.Map.Layers
                .Where(x => x.LayerType.Equals("objectgroup", StringComparison.InvariantCultureIgnoreCase) && x.Name.Equals("hudOverlayLayer", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(x => x.LayerObjects)
                .ToArray();

            // Add GameObjects for each layerObject
            foreach (var layerObject in layerObjects)
            {
                AddHudEntity(layerObject);
            }
        }

        private void AddHudEntity(LayerObject layerObject)
        {
            if (!_gameAssetsProvider.TryGetTileSet(layerObject.Gid, out var tileSet))
            {
                return;
            }

            var hudEntity = _gameObjectManager.CreateGameObject();
            var transform = new TransformComponent(hudEntity)
            {
                Position = new Vector2(layerObject.X, layerObject.Y)
            };
            hudEntity.AddOrUpdateComponent(transform);

            var animation = new AnimatedGraphicsComponent(
                hudEntity,
                tileSet.TileAtlas.Bitmap,
                new[] { layerObject.Gid - tileSet.FirstGid },
                TimeSpan.Zero,
                tileSet.TileAtlas.Columns,
                new Vector2(tileSet.TileAtlas.TileWidth, tileSet.TileAtlas.TileHeight));
            hudEntity.AddOrUpdateComponent(animation);
            hudEntity.AddOrUpdateComponent(new TagComponent(hudEntity, layerObject.Class));
        }

        private void AddGameCharacterEntities(LayerObject layerObject)
        {
            if (!_gameAssetsProvider.TryGetTileSet(layerObject.Gid, out var tileSet))
            {
                return;
            }

            if (tileSet.IsHeroTileSet())
            {
                var position = new Vector2(layerObject.X, layerObject.Y);
                _heroFactory.CreateHero(tileSet, position);
                return;
            }

            if (tileSet.IsEnemyWaspTileSet())
            {
                _enemyFactory.CreateFlyingEnemy(tileSet, new Vector2(layerObject.X, layerObject.Y), _gameAssetsProvider.Map.ToBoundingBox());
                return;
            }
        }
    }
}
