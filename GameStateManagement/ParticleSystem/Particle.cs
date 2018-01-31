using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.ParticleSystem
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public float Lifetime;
        public float ElapsedTime;
        public float Scale;
        public float Rotation;
        public float RotationSpeed;

        public bool Alive
        {
            get { return ElapsedTime < Lifetime; }
        }

        public Particle()
        {
        }

           
        public void Initialize(Vector2 position, Vector2 velocity, Vector2 acceleration,
            float lifetime, float scale, float rotationSpeed)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Acceleration = acceleration;
            this.Lifetime = lifetime;
            this.Scale = scale;
            this.RotationSpeed = rotationSpeed;
            this.ElapsedTime = 0.0f;
            this.Rotation = new Random().Next() * MathHelper.TwoPi;
        }
                
        public void Update(float dt)
        {
            Velocity += Acceleration * dt;
            Position += Velocity * dt;

            Rotation += RotationSpeed * dt;

            ElapsedTime += dt;
        }
    }
}
