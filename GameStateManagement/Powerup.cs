using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace GameStateManagement
{
    public class Powerup:Actor
    {
        String type;
        
        public Powerup(Game game, float x, float y, float z, int typeSetter)
            : base(game)
        {

            switch (typeSetter)
            {
                case 1:
                    type = "SMG";
                    meshName = "MachineGun_PowerUp";
                    break;
                case 2:
                    type = "Mine";
                    meshName = "Mine_PowerUp";
                    break;
            }

            SetWorldPosition(x, y, z);

            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);

        }   
        public String getType()
        {
            return type;
        }
    }
}
