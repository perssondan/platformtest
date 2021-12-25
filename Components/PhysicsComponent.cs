﻿using System.Numerics;
using uwpPlatformer.Constants;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public class PhysicsComponent : ComponentBase, IComponent
    {
        private float _mass;
        private Vector2 _acceleration;
        private Vector2 _velocity;
        private Vector2 _position;

        public PhysicsComponent(GameObject gameObject) : base(gameObject)
        {
            Mass = 1f;
        }

        public void Reset(Vector2 position)
        {
            Position = position;

            Velocity = Vector2.Zero;

            Acceleration = Vector2.Zero;

            ImpulseForce = Vector2.Zero;
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                PreviousPosition = _position;
                _position = value;
            }
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                PreviousVelocity = _velocity;
                _velocity = value;
            }
        }

        public Vector2 Acceleration
        {
            get => _acceleration;
            set
            {
                PreviousAcceleration = _acceleration;
                _acceleration = value;
            }
        }

        public Vector2 PreviousPosition { get; private set; }

        public Vector2 PreviousVelocity { get; private set; }

        public Vector2 PreviousAcceleration { get; private set; }

        public Vector2 GravityForce { get; set; } = new Vector2(0f, PlayerConstants.Gravity);

        public Vector2 LinearMomentum => Mass * Velocity;

        public float Drag { get; set; } = 5f;

        public float Mass
        {
            get => _mass;
            set
            {
                if (_mass == value) return;

                _mass = value;
                MassInverted = 1f / _mass;
            }
        }

        public float MassInverted { get; private set; }

        public Vector2 ImpulseForce { get; set; }
    }
}
