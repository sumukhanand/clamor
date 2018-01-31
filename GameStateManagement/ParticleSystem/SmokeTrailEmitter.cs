using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.ParticleSystem
{
    public class SmokeTrailEmitter : BaseParticleSystem
    {
        public SmokeTrailEmitter(Game game, int effects)
            : base(game, effects)
        {
        }

        protected override void InitializeConstants()
        {
            texture = "Images/smoke";

            minSpeed = 20;
            maxSpeed = 100;
            minAccel = 0;
            maxAccel = 0;
            minLifetime = 5.0f;
            maxLifetime = 7.0f;
            minScale = .5f;
            maxScale = 1.0f;
            minParticles = 7;
            maxParticles = 15;
            minRotSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotSpeed = MathHelper.PiOver4 / 2.0f;

            remainingTime = 0f;
            totalTime = 0.5f;

            blend = BlendState.AlphaBlend;
            DrawOrder = AlphaBlendDrawOrder;
        }

        protected override Vector2 PickRandomDirection()
        {
            float radians = RandomBetween(MathHelper.ToRadians(40), MathHelper.ToRadians(50));
            Vector2 dir = Vector2.Zero;

            dir.Y = -(float)Math.Sin(radians);

            return dir;
        }

        protected override void InitializeParticle(Particle p, Vector2 position)
        {
            p.Acceleration.X += RandomBetween(-20, 20);
            this.position = position;

            base.InitializeParticle(p, position);
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (active)
            {
                remainingTime -= dt;
                if (remainingTime < 0)
                {
                    this.AddParticles(position);
                    remainingTime = totalTime;
                }

                base.Update(gameTime);
            }
        }
    }
}
