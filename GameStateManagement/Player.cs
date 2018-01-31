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
using Utils;
using GameStateManagement.ParticleSystem;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : Actor
    {
        public Boolean alive = false;

        private Game game;
        private GameplayScreen gScreen;
        private GraphicsDeviceManager graphics;

        public List<Missile> missiles = new List<Missile>();
        public List<Mine> mines = new List<Mine>();
        private ExplosionEmitter explosion;

        private Matrix projectionMatrix;
        private Matrix cameraMatrix;

        public float MaxSpeed;
        public int health;
        private Timer timer;
        private int index;

        private Vector3 temp;

        private bool hit;
        public bool Hit
        {
            set
            {
                 hit = value;
                timer.AddTimer("hitTimer", .3f, removeHit, false);
            }
            get
            {
                return hit;
            }
        }

        public List<Gun> guns = new List<Gun>();
        public int currentGun;

        public Player(Game g, GameplayScreen gs, GraphicsDeviceManager gdm, Timer t, int i)
            : base(g)
        {
            game = g;
            gScreen = gs;
            graphics = gdm;
            index = i;
            timer = t;
            meshName = "Player "+i;
            alive = true;
            MaxSpeed = 1500f;
            health = 100;
            guns.Add(new Gun(game, "Pistol", t, i));
            guns.Add(new Gun(game, "SMG", t, i));
            guns.Add(new Gun(game, "Mine", t, i));

            game.Components.Add(guns[0]);
            game.Components.Add(guns[1]);
            game.Components.Add(guns[2]);

            createRotation(new Vector3(1f, 0f, 0f), MathHelper.PiOver2);
            createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);

            fMass = 1f;
            fTerminalVelocity = 500f;

            if (bPhysicsDriven)
            {
                vForce = new Vector3(0f, 0f, 0f);
            }
            else
            {
                velocity = new Vector3(0f, 0f, 0f);
            }

            currentGun = 0;

            explosion = new ExplosionEmitter(game, 1, 0.3f);
            game.Components.Add(explosion);
            explosion.active = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!alive && !explosion.active)
            {
                explosion.active = true;
                paused = true;
                Vector3 twoDCoord = graphics.GraphicsDevice.Viewport.Project(Vector3.Zero, projectionMatrix, cameraMatrix, worldMatrix);
                explosion.AddParticles(new Vector2(twoDCoord.X, twoDCoord.Y));
                timer.AddTimer("killP"+index, .4f, kill, false);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (hit && !paused)
            {
                foreach (ModelMesh mesh in modelMesh.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.AmbientLightColor = new Vector3(1, 0, 0);
                    }
                    mesh.Draw();
                }
            }
        }

        public override void setPaused(Boolean b)
        {
            base.setPaused(b);
            foreach (Missile missile in missiles)
            {
                missile.setPaused(paused);
            }
        }

        public void setMeshName(string meshname)
        {
            meshName = meshname;
        }

        public void rotateLeft(float seconds)
        {
            if (bPhysicsDriven)
            {
                temp.X = 0f;
                temp.Y = 0f;
                temp.Z = 1f;
                createRotation(temp, 2 * MathHelper.Pi * seconds);
            }
            else
                createRotation(temp, 2 * MathHelper.Pi * seconds);
        }

        public void rotateRight(float seconds)
        {
            if (bPhysicsDriven)
            {
                temp.X = 0f;
                temp.Y = 0f;
                temp.Z = 1f;
                createRotation(new Vector3(0f, 0f, 1f), -2 * MathHelper.Pi * seconds);
            }
            else
            {
                temp.X = 0f;
                temp.Y = 0f;
                temp.Z = 1f;
                createRotation(new Vector3(0f, 0f, 1f), -2 * MathHelper.Pi * seconds);
            }
        }

        public void moveDown()
        {
            temp.X = 0f;
                temp.Y = -1f * MaxSpeed;
                temp.Z = 0f;
                velocity = temp;
        }

        public void moveUp()
        {
            temp.X = 0f;
                temp.Y = 1f * MaxSpeed;
                temp.Z = 0f;
                velocity = temp;
        }
        
        public void moveLeft()
        {
            temp.X = -1f * MaxSpeed;
            temp.Y = 0f;
            temp.Z = 0f;
            velocity = temp;
        }

        public void moveRight()
        {
            temp.X = 1f * MaxSpeed;
            temp.Y = 0f;
            temp.Z = 0f;
            velocity = temp;
        }
        
        public void moveJoystick(Vector2 controlVector)
        {
            temp.Y = controlVector.Y * MaxSpeed;
                temp.X = controlVector.X * MaxSpeed;
                temp.Z = 0f;
                velocity = temp;
        }

        public void rotateJoystick(Vector2 controlVector)
        {

            controlVector.Normalize();
            if ((controlVector.X >= 0 || controlVector.X <= 0) || (controlVector.Y >= 0 || controlVector.Y <= 0))
            {
                float angle = (float)Math.Acos(controlVector.Y);
                if (controlVector.X > 0.0f)
                    angle = -angle;
                SnapRotate(angle);
            }

        }

        public void coolDown()
        {
            canFire = true;
            timer.RemoveTimer("missileTimer"+index);
        }

        public void stopMoving()
        {
            if (bPhysicsDriven)
                vForce = Vector3.Zero;
            else
                velocity = Vector3.Zero;
        }

        public void switchWeapon(int i)
        {
            currentGun = i;
        }

        public void fire()
        {
            if (canFire)
            {
                if (guns[currentGun].clipAmmo > 0)
                {
                    canFire = false;
                    guns[currentGun].clipAmmo--;

                    if (currentGun == 0 || currentGun == 1)
                    {
                        //Missile m = new Missile(game, Position + worldMatrix.Forward*2, worldMatrix.Forward, Rotation, index);
                        //gScreen.addMissile(m);
                        gScreen.fire(Position, worldMatrix.Forward, Rotation, index);
                    }
                    else
                    {
                        Mine m = new Mine(game, graphics, timer, Position, worldMatrix.Forward, Rotation, projectionMatrix, cameraMatrix, index);
                        gScreen.addMine(m);
                        timer.AddTimer("mineTimer" + index, 2f, new Utils.TimerDelegate(m.Activate), false);
                    }
                    if (currentGun == 0)
                    {
                        timer.AddTimer("missileTimer" + index, 0.30f, new Utils.TimerDelegate(coolDown), false);
                    }
                    else if (currentGun == 1)
                    {
                        timer.AddTimer("missileTimer" + index, 0.125f, new Utils.TimerDelegate(coolDown), false);
                    }
                    else
                    {
                        timer.AddTimer("missileTimer" + index, 2.5f, new Utils.TimerDelegate(coolDown), false);
                    }
                }
            }

            //gScreen.fire(Position, worldMatrix.Forward, Rotation, index);
        }

        public void kill()
        {
            gScreen.killPlayer(index);
        }

        public void removeHit()
        {
            hit = false;
            timer.RemoveTimer("hitTimer");
        }

        public void setCamera(Matrix p, Matrix c)
        {
            projectionMatrix = p;
            cameraMatrix = c;
        }
    }
}
