using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameStateManagement.ParticleSystem;
using Utils;

namespace GameStateManagement
{
    public class Mine : Actor
    {
        private GraphicsDeviceManager graphics;
        private Timer timer;

        public int playerIndex;
        public bool drawMine;
        public int damage = 50;
        public bool active = false;

        private ExplosionEmitter explosion;

        private Matrix projectionMatrix;
        private Matrix cameraMatrix;

        public Mine(Game game, GraphicsDeviceManager gdm, Timer t, Vector3 position, Vector3 velocity, Quaternion rotation, Matrix p, Matrix c, int index)
            : base(game)
        {
            playerIndex = index;
            meshName = "Mine";
            Random random = new Random();
            bPhysicsDriven = true;
            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);
            drawMine = true;

            timer = t;
            graphics = gdm;
            projectionMatrix = p;
            cameraMatrix = c;

            fMass = 1f;
            fTerminalVelocity = 0f;
            Position = position;
            Rotation = rotation;

            if (bPhysicsDriven)
            {
                vForce = new Vector3(0f, 0f, 0f);
            }
            else
            {
                velocity = new Vector3(0f, 0f, 0f);
            }

            explosion = new ExplosionEmitter(game, 1, 0.3f);
            game.Components.Add(explosion);
            explosion.active = false;

            //GameplayScreen.soundBank.PlayCue("Ship_Missile");
        }

        public void explode()
        {
            explosion.active = true;
            Vector3 twoDCoord = graphics.GraphicsDevice.Viewport.Project(Vector3.Zero, projectionMatrix, cameraMatrix, worldMatrix);
            explosion.AddParticles(new Vector2(twoDCoord.X, twoDCoord.Y));
        }

        public override void Draw(GameTime gameTime)
        {
            if(drawMine)
                base.Draw(gameTime);
        }
        public override void Update(GameTime gameTime)
        {

        }

        public void Activate()
        {
            active = true;
            timer.RemoveTimer("mineTimer" + playerIndex);
        }

        public void Deactivate()
        {
            active = false;
        }
    }
}
