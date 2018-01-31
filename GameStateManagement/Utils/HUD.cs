using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

namespace Utils
{
    public class HUD : DrawableGameComponent
    {
        private ContentManager content;
        private SpriteBatch spriteBatch;


        GraphicsDeviceManager graphics;
        Player[] players;
        bool paused;
        Timer timer;
        SpriteFont gameFont;

        Texture2D p1HUD;
        Texture2D p2HUD;
        Texture2D p3HUD;
        Texture2D p4HUD;
        Texture2D healthGreenHUD;
        Texture2D healthRedHUD;
        Texture2D pistol;
        Texture2D smg;
        Texture2D mine;

        public HUD(Game game, GraphicsDeviceManager g, Player[] p, Timer t, SpriteFont f)
            : base(game)
        {
            content = new ContentManager(game.Services);
            content.RootDirectory = "Content";

            graphics = g;
            players = p;
            timer = t;
            gameFont = f;

            DrawOrder = 1000;
        }

        protected override void LoadContent()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));

            spriteBatch = new SpriteBatch(graphicsService.GraphicsDevice);

            p1HUD = this.content.Load<Texture2D>("Images/p1_hud");
            p2HUD = this.content.Load<Texture2D>("Images/p2_hud");
            p3HUD = this.content.Load<Texture2D>("Images/p3_hud");
            p4HUD = this.content.Load<Texture2D>("Images/p4_hud");
            healthGreenHUD = this.content.Load<Texture2D>("Images/hud_health_green");
            healthRedHUD = this.content.Load<Texture2D>("Images/hud_health_red");
            pistol = this.content.Load<Texture2D>("Images/pistol");
            smg = this.content.Load<Texture2D>("Images/smg");
            mine = this.content.Load<Texture2D>("Images/mine");
        }

        protected override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public void UpdateHUD(Player[] p, bool pause, Timer t)
        {
            players = p;
            paused = pause;
            timer = t;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!paused)
            {
                spriteBatch.Begin();
                Color timerColor;
                if ((int)timer.GetRemainingTime("roundTimer") >= 30)
                {
                    timerColor = Color.Green;
                }
                else if ((int)timer.GetRemainingTime("roundTimer") >= 10)
                {
                    timerColor = Color.Yellow;
                }
                else
                {
                    timerColor = Color.Red;
                }

                spriteBatch.DrawString(gameFont, "Round Time: " + (int)timer.GetRemainingTime("roundTimer"), new Vector2(425, 30), timerColor,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);


                //PLAYER 1
                if (players[0] != null)
                {
                    spriteBatch.Draw(p1HUD, new Rectangle(0, 0, p1HUD.Width, p1HUD.Height), null, Color.White, 0,
                                        new Vector2(0, 0), SpriteEffects.None, 0);
                    if (players[0].health < 25)
                        spriteBatch.Draw(healthRedHUD, new Rectangle(180, 15, (int)(healthRedHUD.Width * (players[0].health / 100f)), healthRedHUD.Height), null, Color.White, 0,
                                            new Vector2(0, 0), SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(healthGreenHUD, new Rectangle(180, 15, (int)(healthGreenHUD.Width * (players[0].health / 100f)), healthGreenHUD.Height), null, Color.White, 0,
                                            new Vector2(0, 0), SpriteEffects.None, 0);

                    spriteBatch.DrawString(gameFont, players[0].guns[players[0].currentGun].clipAmmo + "/" + players[0].guns[players[0].currentGun].ammo, new Vector2(40, 50), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
                    
                    switch (players[0].currentGun)
                    {
                        case 0:
                            spriteBatch.Draw(pistol, new Rectangle(10, 50, 20, 20), Color.White);
                            break;
                        case 1:
                            spriteBatch.Draw(smg, new Rectangle(10, 50, 20, 20), Color.White);
                            break;
                        case 2:
                            spriteBatch.Draw(mine, new Rectangle(10, 50, 20, 20), Color.White);
                            break;
                    }
                }

                //PLAYER 2
                if (players[1] != null)
                {
                    spriteBatch.Draw(p2HUD, new Rectangle(1028, -6, p2HUD.Width, p2HUD.Height), null, Color.White, 0,
                                       new Vector2(p2HUD.Width, 0), SpriteEffects.None, 0);
                    if (players[1].health < 25)
                        spriteBatch.Draw(healthRedHUD, new Rectangle(1010 - healthRedHUD.Width, 12, (int)(healthRedHUD.Width * (players[1].health / 100f)), healthRedHUD.Height), null, Color.White, 0,
                                            new Vector2(0, 0), SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(healthGreenHUD, new Rectangle(1010 - healthGreenHUD.Width, 12, (int)(healthGreenHUD.Width * (players[1].health / 100f)), healthGreenHUD.Height), null, Color.White, 0,
                                            new Vector2(0, 0), SpriteEffects.None, 0);

                    spriteBatch.DrawString(gameFont, players[1].guns[players[1].currentGun].clipAmmo + "/" + players[1].guns[players[1].currentGun].ammo, new Vector2(680, 50), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

                    switch (players[1].currentGun)
                    {
                        case 0:
                            spriteBatch.Draw(pistol, new Rectangle(650, 50, 20, 20), Color.White);
                            break;
                        case 1:
                            spriteBatch.Draw(smg, new Rectangle(650, 50, 20, 20), Color.White);
                            break;
                        case 2:
                            spriteBatch.Draw(mine, new Rectangle(650, 50, 20, 20), Color.White);
                            break;
                    }
                }

                //PLAYER 3
                if (players[2] != null)
                {
                    spriteBatch.Draw(p3HUD, new Rectangle(0, 768, p3HUD.Width, p3HUD.Height), null, Color.White, 0,
                                           new Vector2(0, p3HUD.Height), SpriteEffects.None, 0);
                    if (players[2].health < 25)
                        spriteBatch.Draw(healthRedHUD, new Rectangle(180, 755, (int)(healthRedHUD.Width * (players[2].health / 100f)), healthRedHUD.Height), null, Color.White, 0,
                                            new Vector2(0, healthRedHUD.Height), SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(healthGreenHUD, new Rectangle(180, 755, (int)(healthGreenHUD.Width * (players[2].health / 100f)), healthGreenHUD.Height), null, Color.White, 0,
                                            new Vector2(0, healthGreenHUD.Height), SpriteEffects.None, 0);

                    spriteBatch.DrawString(gameFont, players[2].guns[players[2].currentGun].clipAmmo + "/" + players[2].guns[players[2].currentGun].ammo, new Vector2(40, 690), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

                    switch (players[2].currentGun)
                    {
                        case 0:
                            spriteBatch.Draw(pistol, new Rectangle(10, 690, 20, 20), Color.White);
                            break;
                        case 1:
                            spriteBatch.Draw(smg, new Rectangle(10, 690, 20, 20), Color.White);
                            break;
                        case 2:
                            spriteBatch.Draw(mine, new Rectangle(10, 690, 20, 20), Color.White);
                            break;
                    }
                }

                //PLAYER 4
                if (players[3] != null)
                {
                    spriteBatch.Draw(p4HUD, new Rectangle(1028, 768, p4HUD.Width, p4HUD.Height), null, Color.White, 0,
                                           new Vector2(p4HUD.Width, p4HUD.Height), SpriteEffects.None, 0);
                    if (players[3].health < 25)
                        spriteBatch.Draw(healthRedHUD, new Rectangle(1010 - healthRedHUD.Width, 753, (int)(healthRedHUD.Width * (players[3].health / 100f)), healthRedHUD.Height), null, Color.White, 0,
                                            new Vector2(0, healthRedHUD.Height), SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(healthGreenHUD, new Rectangle(1010 - healthGreenHUD.Width, 753, (int)(healthGreenHUD.Width * (players[3].health / 100f)), healthGreenHUD.Height), null, Color.White, 0,
                                            new Vector2(0, healthGreenHUD.Height), SpriteEffects.None, 0);

                    spriteBatch.DrawString(gameFont, players[3].guns[players[3].currentGun].clipAmmo + "/" + players[3].guns[players[3].currentGun].ammo, new Vector2(680, 690), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

                    switch (players[3].currentGun)
                    {
                        case 0:
                            spriteBatch.Draw(pistol, new Rectangle(650, 690, 20, 20), Color.White);
                            break;
                        case 1:
                            spriteBatch.Draw(smg, new Rectangle(650, 690, 20, 20), Color.White);
                            break;
                        case 2:
                            spriteBatch.Draw(mine, new Rectangle(650, 690, 20, 20), Color.White);
                            break;
                    }
                }
                spriteBatch.End();
            }
            
        }
        public void setPaused(bool p) {
            paused = p;
        }
    }
}
