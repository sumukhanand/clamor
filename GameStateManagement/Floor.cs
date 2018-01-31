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
   public class Floor : Actor
    {
       public Floor(Game game) 
           : base(game)
       {
            meshName = "Floor";
            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);
            //createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);

            lighting = false;

            SetWorldPosition(0, 0, 0);
       }

       public override void Update(GameTime gameTime)
       {

       }
    }
    

}
