using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.ParticleSystem
{
    class ExplosionSmokeEmitter : BaseParticleSystem
    {
        public ExplosionSmokeEmitter(Game game, int howManyEffects)
            : base(game, howManyEffects)
        {            
        }

        protected override void InitializeConstants()
        {
            texture = "Images/smoke";

            minSpeed = 20;
            maxSpeed = 200;
            minAccel = -10;
            maxAccel = -50;
            minLifetime = .5f;
            maxLifetime = 1.0f;
            minScale = 1.0f;
            maxScale = 2.0f;
            minParticles = 10;
            maxParticles = 20;
            minRotSpeed = -MathHelper.PiOver4;
            maxRotSpeed = MathHelper.PiOver4;

            remainingTime = 0;
            totalTime = 1;

			blend = BlendState.AlphaBlend;
            DrawOrder = AlphaBlendDrawOrder;
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
