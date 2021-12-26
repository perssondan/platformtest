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
            var velocity = gameObject.PhysicsComponent.Velocity;
            WalkHandler(gameObject, userInputs, (float)deltatime.TotalSeconds, velocity);
            JumpHandler(gameObject, userInputs, deltatime, velocity);
        }

        private void JumpHandler(GameObject gameObject, UserInput userInputs, TimeSpan timeSpan, Vector2 velocity)
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
            if (!IsVerticallyStationary(velocity)) return;

            playerComponent.JumpPressedAt = TimeSpan.Zero;

            AddJumpImpulse(gameObject, velocity, (float)timeSpan.TotalSeconds);
        }

        private bool IsVerticallyStationary(Vector2 velocity)
        {
            
            return velocity.Y < .5f && velocity.Y > -.5f;
        }

        private void WalkHandler(GameObject gameObject, UserInput userInputs, float deltaTime, Vector2 velocity)
        {
            var horizontalWalkOrientation = GetHorizontalWalkOrientationFromUserInput(userInputs);
            var verticalWalkOrientation = GetVerticalWalkOrientationFromUserInput(userInputs);
            UpdateWalkAnimation(gameObject.GraphicsComponent, gameObject.GetComponent<PlayerComponent>(), horizontalWalkOrientation);
            //AddWalkImpulse(gameObject.PhysicsComponent, walkOrientation, deltaTime, velocity);
            AddWalkImpulseSimple(gameObject.PhysicsComponent, new Vector2(horizontalWalkOrientation, verticalWalkOrientation));
        }

        private float GetHorizontalWalkOrientationFromUserInput(UserInput userInputs)
        {
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

        private void AddJumpImpulse(GameObject gameObject, Vector2 velocity, float deltaTime)
        {
            var requiredForceScalar = ResolveRequiredForce(PlayerConstants.InitialVerticalVelocity, velocity.Y, deltaTime);
            if (float.IsNaN(requiredForceScalar)) return;

            var jumpImpulseForce = Vector2.UnitY * requiredForceScalar;
            gameObject.PhysicsComponent.ImpulseForce += jumpImpulseForce;

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

        private void AddWalkImpulse(PhysicsComponent physicsComponent, float orientation, float deltaTime, Vector2 velocity)
        {
            var requiredForceScalar = ResolveRequiredForce((orientation * PlayerConstants.InitialHorizontalVelocity), velocity.X, deltaTime);
            if (float.IsNaN(requiredForceScalar)) return;

            var walkImpulseForce = Vector2.UnitX * requiredForceScalar;
            physicsComponent.ImpulseForce += walkImpulseForce;
        }

        private static float WalkImpuls = 2000f;
        /// <summary>
        /// This just add impulse, left or right, nothing more.
        /// </summary>
        /// <param name="physicsComponent"></param>
        /// <param name="orientation"></param>
        /// <param name="deltaTime"></param>
        /// <param name="velocity"></param>
        private void AddWalkImpulseSimple(PhysicsComponent physicsComponent, Vector2 orientation)
        {
            physicsComponent.ImpulseForce += (orientation * WalkImpuls);
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
