using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Missile : Actor
    {
        public int playerIndex;
        public int damage = 20;
        public Boolean active = false;

        public Missile(Game game, Vector3 position, Vector3 v, Quaternion rotation, int index)
            : base(game)
        {
            playerIndex = index;
            meshName = "Missile";
            Random random = new Random();
            bPhysicsDriven = false;
            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);



            fMass = 1f;
            fTerminalVelocity = 2000f;
            Position = position;
            Rotation = rotation;

            if (bPhysicsDriven)
            {
                vForce = -v * 100000;
            }
            else
            {
                velocity = -v * 3000;
            }
        }

        public void setMissile(Vector3 position, Vector3 v, Quaternion rotation, int i)
        {
            playerIndex = i;
            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);
            Position = position;
            Rotation = rotation;
            meshName = "Missile";

            if (bPhysicsDriven)
            {
                vForce = -v * 100000;
            }
            else
            {
                velocity = -v * 3000;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (active)
            {
                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (active)
            {
                base.Draw(gameTime);
            }
        }
    }
     
}
