using GamesLibrary.Models;
using GamesLibrary.Systems;
using GamesLibrary.Utilities;
using System;
using System.Linq;
using System.Numerics;
using uwpPlatformer.Components;
using uwpPlatformer.Components.Particles;
using uwpPlatformer.Constants;
using uwpPlatformer.EventArguments;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public class PlayerSystem : ISystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly IGameObjectManager _gameObjectManager;

        public PlayerSystem(IEventSystem eventSystem, IGameObjectManager gameObjectManager)
        {
            _eventSystem = eventSystem;
            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(PlayerSystem);

        public void Update(TimingInfo timingInfo)
        {
            _gameObjectManager.GameObjects
                .Where(gameObject => gameObject.Has<PlayerComponent>())
                .ForEach(gameObject =>
                {
                    UpdatePlayerGameObject(gameObject, timingInfo.ElapsedTime);
                });
        }

        // InputComponent,
        // PlayerComponent,
        // PhysicsComponent,
        // AnimatedGraphicsComponent,
        // ParticleEmitterComponent

        public void Init()
        {
        }

        private void UpdatePlayerGameObject(GameObject gameObject, TimeSpan deltatime)
        {
            var userInputs = gameObject.GetComponent<InputComponent>()?.UserInputs ?? UserInput.None;
            RampDownHorizontalVelocity(gameObject, userInputs);
            WalkLeftHandler(gameObject, userInputs);
            WalkRightHandler(gameObject, userInputs);
            JumpHandler(gameObject, userInputs, deltatime);
            UpdateAnimation(gameObject);
        }

        private void UpdateAnimation(GameObject gameObject)
        {
            (PlayerComponent playerComponent, AnimatedGraphicsComponent graphicsComponent, PhysicsComponent physicsComponent) = gameObject.GetComponents<PlayerComponent, AnimatedGraphicsComponent, PhysicsComponent>();

            var walkOrientation = physicsComponent.Velocity.X < 0f ? HorizontalMovement.Left : physicsComponent.Velocity.X > 0f ? HorizontalMovement.Right : HorizontalMovement.Stationary;
            switch (walkOrientation)
            {
                case HorizontalMovement.Left:
                    graphicsComponent.InvertTile = true;
                    graphicsComponent.SourceSpriteIndexes = PlayerComponent.WalkSourceSpriteIndexes;
                    break;
                case HorizontalMovement.Right:
                    graphicsComponent.InvertTile = false;
                    graphicsComponent.SourceSpriteIndexes = PlayerComponent.WalkSourceSpriteIndexes;
                    break;
                default:
                    graphicsComponent.InvertTile = false;
                    graphicsComponent.SourceSpriteIndexes = PlayerComponent.StationarySourceSpriteIndexes;
                    break;
            }
        }

        private bool IsJumpButtonPressed(PlayerComponent playerComponent, UserInput userInputs, TimeSpan timeSpan)
        {
            // No jump input
            if ((userInputs & UserInput.Jump) != UserInput.Jump)
            {
                playerComponent.IsJumpButtonPressed = false;
                playerComponent.JumpPressedAt = TimeSpan.Zero;
                return false;
            }

            // Subtract time from when we first pressed, in order to jump a bit later
            playerComponent.JumpPressedAt -= timeSpan;

            // If this is the first frame we press jump button, reset the jump countdown time.
            // This also prevents from the player to holding the jump button in and jump continiously.
            if (playerComponent.IsJumpButtonPressed == false)
            {
                playerComponent.JumpPressedAt = playerComponent.JumpPressedRememberTime;
            }

            playerComponent.IsJumpButtonPressed = true;

            return playerComponent.JumpPressedAt.TotalMilliseconds > 0f;
        }

        private void JumpHandler(GameObject gameObject, UserInput userInputs, TimeSpan timeSpan)
        {
            var playerComponent = gameObject.GetComponent<PlayerComponent>();
            // If we didn't press the jump button but are still on the way up
            // we could double the gravity force to make a short jump.
            if (!IsJumpButtonPressed(playerComponent, userInputs, timeSpan)) return;

            var physicsComponent = gameObject.GetComponent<PhysicsComponent>();

            //Only jump when grounded
            if (!IsVerticallyStationary(physicsComponent.Velocity)) return;

            playerComponent.JumpPressedAt = TimeSpan.Zero;

            AddJumpImpulse(physicsComponent);
            AddDustEmitter(gameObject);
            _eventSystem.Send(this, new JumpEvent(gameObject));
        }

        private bool IsVerticallyStationary(Vector2 velocity)
        {
            return Math.Abs(velocity.Y) < PlayerConstants.VerticallyStationaryThreshold;
        }

        private void WalkLeftHandler(GameObject gameObject, UserInput userInputs)
        {
            if ((userInputs & UserInput.Left) != UserInput.Left) return;


            (PlayerComponent playerComponent, PhysicsComponent physicsComponent)
    components = gameObject.GetComponents<PlayerComponent, PhysicsComponent>();

            var newVx = GameMath.Lerp(components.physicsComponent.Velocity.X, -PlayerConstants.V0x, 0.5f);
            components.physicsComponent.Velocity = components.physicsComponent.Velocity * Vector2.UnitY + new Vector2(newVx, 0f);
        }

        private void RampDownHorizontalVelocity(GameObject gameObject, UserInput userInputs)
        {
            (PlayerComponent playerComponent, PhysicsComponent physicsComponent)
                components = gameObject.GetComponents<PlayerComponent, PhysicsComponent>();

            var horizontalWalkOrientation = GetHorizontalWalkOrientationFromUserInput(userInputs);

            if (horizontalWalkOrientation != HorizontalMovement.Stationary) return;

            var lerpFactor = IsVerticallyStationary(components.physicsComponent.Velocity) ? 0.15f : 0f;

            var newVx = GameMath.Lerp(components.physicsComponent.Velocity.X, 0f, lerpFactor);
            components.physicsComponent.Velocity = components.physicsComponent.Velocity * Vector2.UnitY + new Vector2(newVx, 0f);
        }

        private void WalkRightHandler(GameObject gameObject, UserInput userInputs)
        {
            if ((userInputs & UserInput.Right) != UserInput.Right) return;

            (PlayerComponent playerComponent, PhysicsComponent physicsComponent)
                components = gameObject.GetComponents<PlayerComponent, PhysicsComponent>();

            var newVx = GameMath.Lerp(components.physicsComponent.Velocity.X, PlayerConstants.V0x, 0.5f);
            components.physicsComponent.Velocity = components.physicsComponent.Velocity * Vector2.UnitY + new Vector2(newVx, 0f);
        }

        private HorizontalMovement GetHorizontalWalkOrientationFromUserInput(UserInput userInputs)
        {
            if ((userInputs & UserInput.Right) == UserInput.Right) return HorizontalMovement.Right;

            if ((userInputs & UserInput.Left) == UserInput.Left) return HorizontalMovement.Left;

            return HorizontalMovement.Stationary;
        }

        private void AddJumpImpulse(PhysicsComponent physicsComponent)
        {
            physicsComponent.Velocity += Vector2.UnitY * PlayerConstants.V0y;
        }

        private static void AddDustEmitter(GameObject gameObject)
        {
            if (gameObject.Has<ParticleEmitterComponent>()) return;

            if (!gameObject.TryGetComponent<ColliderComponent>(out var colliderComponent)) return;

            gameObject.AddOrUpdateComponent(new ParticleEmitterComponent(
                gameObject,
                ParticleTemplateType.Dust,
                colliderComponent.BoundingBox.BottomCenterOffset()));
        }

        protected enum HorizontalMovement
        {
            Left = -1,
            Stationary = 0,
            Right = 1,
        }
    }
}
