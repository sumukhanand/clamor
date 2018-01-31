using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.ParticleSystem
{
    public abstract class BaseParticleSystem : DrawableGameComponent
    {
        public const int AlphaBlendDrawOrder = 100;
        public const int AdditiveDrawOrder = 200;

        protected Game game;

        private Texture2D sprite;

        private Vector2 origin;

        private int effects;

        private Particle[] particles;

        public Queue<Particle> FreeParticles;

        protected int minParticles;
        protected int maxParticles;

        protected string texture;

        protected float minSpeed;
        protected float maxSpeed;
        protected float minAccel;
        protected float maxAccel;
        protected float minRotSpeed;
        protected float maxRotSpeed;
        protected float minLifetime;
        protected float maxLifetime;
        protected float minScale;
        protected float maxScale;

        protected float remainingTime = 0.0f;
        protected float totalTime = 0.0f;

        public Vector2 position;

        public Boolean active;

        protected BlendState blend;

        protected BaseParticleSystem(Game game, int effects)
            : base(game)
        {
            this.game = game;
            this.effects = effects;
        }

        public override void Initialize()
        {
            InitializeConstants();

            particles = new Particle[effects * maxParticles];
            FreeParticles = new Queue<Particle>(effects * maxParticles);

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle();
                FreeParticles.Enqueue(particles[i]);
            }

            base.Initialize();
        }

        protected abstract void InitializeConstants();

        protected override void LoadContent()
        {
            if (!string.IsNullOrEmpty(texture))
            {
                sprite = game.Content.Load<Texture2D>(texture);
                origin.X = sprite.Width / 2;
                origin.Y = sprite.Height / 2;
            }

            base.LoadContent();
        }

        public void AddParticles(Vector2 position)
        {
            int numParticles = new Random().Next(minParticles, maxParticles);

            for (int i = 0; i < numParticles && FreeParticles.Count > 0; i++)
            {
                Particle p = FreeParticles.Dequeue();
                InitializeParticle(p, position);
            }
        }

        protected virtual void InitializeParticle(Particle p, Vector2 position)
        {
            Vector2 direction = PickRandomDirection();

            float velocity = RandomBetween(minSpeed, maxSpeed);
            float accel = RandomBetween(minAccel, maxAccel);
            float lifetime = RandomBetween(minLifetime, maxLifetime);
            float scale = RandomBetween(minScale, maxScale);
            float rotSpeed = RandomBetween(minRotSpeed, maxRotSpeed);

            p.Initialize(position, velocity * direction, accel * direction, lifetime, scale, rotSpeed);
        }

        protected virtual Vector2 PickRandomDirection()
        {
            float angle = RandomBetween(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)new Random().NextDouble() * (max - min);
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Particle p in particles)
            {
                if (p.Alive)
                {
                    p.Update(dt);
                    if (!p.Alive)
                    {
                        FreeParticles.Enqueue(p);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(game.GraphicsDevice);

            spriteBatch.Begin(SpriteSortMode.Deferred, blend);

            foreach (Particle p in particles)
            {
                if (p.Alive)
                {
                    float normalizedLifetime = p.ElapsedTime / p.Lifetime;
                    float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
                    Color color = Color.White * alpha;
                    float scale = p.Scale * (.75f + .25f * normalizedLifetime);

                    spriteBatch.Draw(sprite, p.Position, null, color, p.Rotation, origin, scale, SpriteEffects.None, 0.0f);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
