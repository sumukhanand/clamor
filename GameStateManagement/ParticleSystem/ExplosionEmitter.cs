using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.ParticleSystem
{
    class ExplosionEmitter : BaseParticleSystem
    {
        public ExplosionEmitter(Game game, int howManyEffects, float scale)
            : base(game, howManyEffects)
        {
            maxScale = scale;
            minScale = scale/3;
        }

        protected override void InitializeConstants()
        {
            texture = "Images/explosion";

            minSpeed = 40;
            maxSpeed = 500;
            minAccel = 0;
            maxAccel = 0;
            minLifetime = .1f;
            maxLifetime = 0.4f;
            //minScale = .3f;
            //maxScale = 1.0f;
            minParticles = 20;
            maxParticles = 25;
            minRotSpeed = -MathHelper.PiOver4;
            maxRotSpeed = MathHelper.PiOver4;

            remainingTime = 0;
            totalTime = 10;

            blend = BlendState.Additive;
            DrawOrder = AdditiveDrawOrder;
        }

        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            remainingTime = totalTime;

            base.InitializeParticle(p, where);

            p.Acceleration = -p.Velocity / p.Lifetime;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (active)
            {
                remainingTime -= dt;
                if (remainingTime < 0)
                {
                    //this.AddParticles(position);
                    //remainingTime = totalTime;
                    remainingTime = 0;
                    active = false;
                }

                base.Update(gameTime);
            }
        }
    }
}
