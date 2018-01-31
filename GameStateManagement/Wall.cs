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
    public class Wall : Actor
    {
        public Wall(Game game, float x, float y, float z, int type)
            : base(game)
        {
            switch (type)
            {
                case 1:
                    meshName = "Wall_S";
                    break;
                case 2:
                    meshName = "Wall_M";
                    break;
                case 3:
                    meshName = "Wall_L";
                    break;
                
            }
            
            SetWorldPosition(x, y, z);
            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
