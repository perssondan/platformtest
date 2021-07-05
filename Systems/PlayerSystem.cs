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
            var userInputs = gameObject.InputComponent.UserInputs;
            WalkHandler(gameObject, userInputs, (float)deltatime.TotalSeconds);
            JumpHandler(gameObject, userInputs, deltatime);
        }

        private void JumpHandler(GameObject gameObject, UserInput userInputs, TimeSpan timeSpan)
        {
            var playerComponent = gameObject.GetComponent<PlayerComponent>();
            playerComponent.JumpPressedAt -= timeSpan;

            if ((userInputs & UserInput.Jump) == UserInput.Jump)
            {
                if (playerComponent.IsJumpButtonPressed == false)
                {
                    playerComponent.IsJumpButtonPressed = true;
                    playerComponent.JumpPressedAt = playerComponent.JumpPressedRememberTime;
                }
            }
            else
            {
                playerComponent.IsJumpButtonPressed = false;
                playerComponent.JumpPressedAt = TimeSpan.Zero;
            }

            if (playerComponent.JumpPressedAt.TotalMilliseconds <= 0f) return;

            //Only jump when grounded
            if (!IsVerticallyStationary(gameObject.PhysicsComponent.Velocity)) return;

            playerComponent.JumpPressedAt = TimeSpan.Zero;

            AddJumpImpulse(gameObject, gameObject.PhysicsComponent, (float)timeSpan.TotalSeconds);
        }

        private bool IsVerticallyStationary(Vector2 velocity)
        {
            return velocity.Y < .5f && velocity.Y > -.5f;
        }

        private void WalkHandler(GameObject gameObject, UserInput userInputs, float deltaTime)
        {
            var walkOrientation = GetWalkOrientationFromUserInput(userInputs);
            UpdateWalkAnimation(gameObject.GraphicsComponent, gameObject.GetComponent<PlayerComponent>(), walkOrientation);
            AddWalkImpulse(gameObject.PhysicsComponent, walkOrientation, deltaTime);
        }

        private float GetWalkOrientationFromUserInput(UserInput userInputs)
        {
            if ((userInputs & UserInput.Right) == UserInput.Right) return 1f;

            if ((userInputs & UserInput.Left) == UserInput.Left) return -1f;

            return 0f;
        }

        private void AddJumpImpulse(GameObject gameObject, PhysicsComponent physicsComponent, float deltaTime)
        {
            var requiredForceScalar = ResolveRequiredForce(PlayerConstants.InitialVerticalVelocity, physicsComponent.Velocity.Y, deltaTime);
            if (float.IsNaN(requiredForceScalar)) return;

            var jumpImpulseForce = Vector2.UnitY * requiredForceScalar;
            physicsComponent.ImpulseForce += jumpImpulseForce;

            AddDustEmitter(gameObject);
            _eventSystem.Send(this, new JumpEvent(gameObject));
        }

        private static void AddDustEmitter(GameObject gameObject)
        {
            if (gameObject.Has<ParticleEmitterComponent>()) return;

            gameObject.AddOrUpdateComponent(new ParticleEmitterComponent(
                gameObject,
                ParticleTemplateType.Dust,
                gameObject.ColliderComponent.BoundingBox.BottomCenterOffset()));
        }

        private void AddWalkImpulse(PhysicsComponent physicsComponent, float orientation, float deltaTime)
        {
            var requiredForceScalar = ResolveRequiredForce((orientation * PlayerConstants.InitialHorizontalVelocity), physicsComponent.Velocity.X, deltaTime);
            if (float.IsNaN(requiredForceScalar)) return;

            var walkImpulseForce = Vector2.UnitX * requiredForceScalar;
            physicsComponent.ImpulseForce += walkImpulseForce;
        }

        private float ResolveRequiredForce(float wantedVelocity, float currentVelocity, float deltaTime)
        {
            var requiredHorizontalVelocity = wantedVelocity - currentVelocity;
            var requiredForce = requiredHorizontalVelocity / deltaTime;

            return requiredForce;
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
    }
}
