using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Utils;

namespace GameStateManagement
{
    public class Gun : Actor
    {
        public String type;
        public int ammo;
        public int maxAmmo;
        public int damage;
        public int firespeed;
        public int clipSize;
        public int clipAmmo;
        public bool drawGun;
        Timer timer;
        bool reloading;
        int playerIndex;


        public Gun(Game game, String type, Timer t, int i)
            : base(game)
        {
            this.type = type;
            reloading = false;
            timer = t;
            if (this.type.Equals("Pistol"))
            {
                //meshName = null;
                ammo = 98; // 
                damage = 20;
                clipSize = 7;
                clipAmmo = 7;
                playerIndex = i;
            }

            else if (this.type.Equals("SMG"))
            {
                meshName = "MachineGun";
                ammo = 0;
                maxAmmo = 100;
                damage = 10;
                clipSize = 25;
                clipAmmo = 0;
            }
            else if (this.type.Equals("Mine"))
            {
                //meshName = null;
                ammo = 0;
                maxAmmo = 4;
                damage = 60;
                clipSize = 1;
                clipAmmo = 0;
            }

            //drawGun = true;
        }

        public void addAmmo(String t)
        {
            if (type == t)
            {
                if (t.Equals("Mine"))
                {
                    ammo += 2;
                }
                else if (t.Equals("SMG"))
                {
                    ammo += 100;
                }
                if (ammo > maxAmmo)
                {
                    ammo = maxAmmo;
                }
            }
        }
        public void reload()
        {
            if (reloading == false)
            {
                reloading = true;
                if (timer != null)
                {
                    timer.AddTimer("reloadTimer" + playerIndex, 0.5f, reloadCool, false);
                }
            }
        }

        public void reloadCool()
        {
            if (ammo - (clipSize - clipAmmo) >= 0)
            {
                ammo -= clipSize - clipAmmo;
                clipAmmo = clipSize;
            }
            else if (ammo - (clipSize - clipAmmo) < 0 && ammo != 0)
            {
                clipAmmo += ammo;
                ammo = 0;
            }
            reloading = false;
            timer.RemoveTimer("reloadTimer" + playerIndex);
        }

        public override void Draw(GameTime gameTime)
        {
            if (drawGun)
                base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (clipAmmo == 0 && ammo != 0)
            {
                reload();
            }
        }
    }
}