using GamesLibrary.Models;
using GamesLibrary.Systems;
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

        private void UpdatePlayerGameObject(GameObject gameObject, TimeSpan deltatime)
        {
            var userInputs = gameObject.GetComponent<InputComponent>()?.UserInputs ?? UserInput.None;
            WalkHandler(gameObject, userInputs, deltatime);
            JumpHandler(gameObject, userInputs, deltatime);
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

        private void WalkHandler(GameObject gameObject, UserInput userInputs, TimeSpan deltaTime)
        {
            var components = gameObject.GetComponents<PlayerComponent, PhysicsComponent, AnimatedGraphicsComponent>();
            var playerComponent = components.Item1;
            var physicsComponent = components.Item2;
            var graphicsComponent = components.Item3;
            var horizontalWalkOrientation = GetHorizontalWalkOrientationFromUserInput(playerComponent, userInputs, deltaTime);
            UpdateWalkAnimation(graphicsComponent, playerComponent, horizontalWalkOrientation);
            AddWalkImpulse(physicsComponent, horizontalWalkOrientation, deltaTime);
        }

        private float GetHorizontalWalkOrientationFromUserInput(PlayerComponent playerComponent, UserInput userInputs, TimeSpan timeSpan)
        {
            playerComponent.WalkPressedAt -= timeSpan;

            if ((userInputs & UserInput.Right) == UserInput.Right) return 1f;

            if ((userInputs & UserInput.Left) == UserInput.Left) return -1f;

            return 0f;
        }

        private float GetVerticalWalkOrientationFromUserInput(UserInput userInputs)
        {
            if ((userInputs & UserInput.Down) == UserInput.Down) return 1f;

            if ((userInputs & UserInput.Up) == UserInput.Up) return -1f;

            return 0f;
        }

        private void AddJumpImpulse(PhysicsComponent physicsComponent)
        {
            physicsComponent.Velocity += Vector2.UnitY * PlayerConstants.Vy;
        }

        private void AddWalkImpulse(PhysicsComponent physicsComponent, float orientation, TimeSpan deltaTime)
        {
            if (!IsVerticallyStationary(physicsComponent.Velocity))
            {
                // Only half velocity to add
            }

            if (orientation == 0f)
            {
                // TODO: We need to know when walk impuls or velocity is finished and restore the initial velocit we added
                physicsComponent.Velocity *= Vector2.UnitY;
            }
            else
            {
                physicsComponent.Velocity += Vector2.UnitX * orientation * PlayerConstants.Vx;
            }
        }

        private void UpdateWalkAnimation(AnimatedGraphicsComponent graphicsComponent, PlayerComponent playerComponent, float walkOrientation)
        {
            if (walkOrientation < 0f)
            {
                graphicsComponent.InvertTile = true;
                graphicsComponent.SourceRects = playerComponent.WalkSourceRects;
            }
            else if (walkOrientation > 0f)
            {
                graphicsComponent.InvertTile = false;
                graphicsComponent.SourceRects = playerComponent.WalkSourceRects;
            }
            else
            {
                graphicsComponent.InvertTile = false;
                graphicsComponent.SourceRects = playerComponent.StaticSourceRects;
            }
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
    }
}
