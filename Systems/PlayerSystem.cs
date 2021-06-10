﻿using GamesLibrary.Models;
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
    public class PlayerSystem : SystemBase<PlayerSystem>
    {
        private readonly IEventSystem _eventSystem;
        private readonly IGameObjectManager _gameObjectManager;

        public PlayerSystem(IEventSystem eventSystem, IGameObjectManager gameObjectManager)
        {
            _eventSystem = eventSystem;
            _gameObjectManager = gameObjectManager;
        }

        public override void Update(TimingInfo timingInfo)
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
            WalkHandler(gameObject, userInputs);
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
            if (!IsVerticallyStationary(gameObject)) return;

            playerComponent.JumpPressedAt = TimeSpan.Zero;

            Jump(gameObject);
        }

        private bool IsVerticallyStationary(GameObject gameObject)
        {
            return gameObject.TransformComponent.Velocity.Y < .1f && gameObject.TransformComponent.Velocity.Y > -.1f;
        }

        private void WalkHandler(GameObject gameObject, UserInput userInputs)
        {
            var walkOrientation = GetWalkOrientationFromUserInput(userInputs);
            UpdateWalkAnimation(gameObject.GraphicsComponent, gameObject.GetComponent<PlayerComponent>(), walkOrientation);
            Walk(gameObject, walkOrientation);
        }

        private float GetWalkOrientationFromUserInput(UserInput userInputs)
        {
            if ((userInputs & UserInput.Right) == UserInput.Right) return 1f;

            if ((userInputs & UserInput.Left) == UserInput.Left) return -1f;

            return 0f;
        }

        private void Jump(GameObject gameObject)
        {
            gameObject.TransformComponent.Velocity += gameObject.GetComponent<PlayerComponent>().InitialJumpVelocity;
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

        private void Walk(GameObject gameObject, float orientation)
        {
            var transformComponent = gameObject.TransformComponent;
            if (orientation > 0f || orientation < 0f)
            {
                var horizontalVector = new Vector2(orientation * PlayerConstants.InitialHorizontalVelocity, 0f);
                transformComponent.Velocity = transformComponent.Velocity * Vector2.UnitY + horizontalVector;
            }
            else if ((transformComponent.Velocity.X < 0f && gameObject.PhysicsComponent.Gravity.X < 0)
                || (transformComponent.Velocity.X > 0f && gameObject.PhysicsComponent.Gravity.X > 0))
            {
                transformComponent.Velocity *= Vector2.UnitY;
            }
            else
            {
                transformComponent.Velocity *= Vector2.UnitY;
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
    }
}
