#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using GameStateManagement.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Utils;
using Microsoft.Xna.Framework.Media;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        /*
         * SOUND ENGINE RESOURCES
         */
        //private static AudioEngine audioEngine = new AudioEngine("Content/Sounds.xgs");
        //private static WaveBank waveBank = new WaveBank(audioEngine, "Content/XNAsteroids Waves.xwb");
        //public static SoundBank soundBank = new SoundBank(audioEngine, "Content/XNAsteroids Cues.xsb");

        /*
         * SYSTEM RESOURCES
         */
        GraphicsDeviceManager graphics;
        ContentManager content;
        SpriteFont gameFont;
        Random random = new Random();
        Boolean checkForWon = false;

        /*
         * LIGHTING RESOURCES
         */
       
        public static Vector3 ambientLight = new Vector3(.1f, 0f, 0f);
        public static Vector3 specularColor = new Vector3(.2f, .2f, .2f);
        public static float specularPower = 0.3f;
        public static Vector3 lightDirection = new Vector3(-2.0f, -2.0f, -1.0f);
        public static Vector3 diffuseColor = new Vector3(0.55f, 0.5f, 0.5f);

        /*
         * PLAYER RESOURCES
         */
        public int totalPlayers = 0;
        public int currentPlayers = 0;
        public Player[] players = new Player[4];

        /*
         * CAMERA RESOURCES
         */
        float centerX = 0;
        float centerY = 0;
        public static Matrix cameraMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 2000.0f), Vector3.Zero, Vector3.UnitY);
        public static Matrix projectionMatrix = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 20000.0f);
        //Vector3 twoDCoord;

        /*
         * GAMEPLAY RESOURCES
         */
        private Queue<Missile> missileBuffer = new Queue<Missile>();
        private List<Missile> missiles = new List<Missile>();
        private List<Mine> mines = new List<Mine>();
        HashSet<Missile> missilesToRemove = new HashSet<Missile>();
        HashSet<Powerup> powerupsToRemove = new HashSet<Powerup>();
        HashSet<Mine> minesToRemove = new HashSet<Mine>();
        private Missile mTemp;
        public float roundTime = 5;
        public Utils.Timer timer = new Utils.Timer();

        /*
         * LEVEL RESOURCES
        */

        //private Floor floor;
        private Wall wall;
        public List<Wall> walls = new List<Wall>();
        private Powerup powerup;
        public List<Powerup> powerups = new List<Powerup>();
        /*
         * IMAGES
         */
        HUD hud;

        #endregion


        #region Initialization

        public GameplayScreen(int n, int time, GraphicsDeviceManager g)
        {
            name = "game";
            MediaPlayer.Stop();
            MediaPlayer.Play(GameStateManagementGame.gameplayMusic);

            //SET UP ROUND TIME
            this.roundTime = time;
            

            //INITIALIZE FADE TIMES
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //INITIALIZE TOTAL PLAYERS
            totalPlayers = n;
            currentPlayers = n;
            timer.AddTimer("roundTimer", roundTime, new Utils.TimerDelegate(endRoundDraw), false);

            graphics = g;

            paused = false;
        }

        public override void LoadContent()
        {

            //GET CONTENT
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            //GET MAIN FONT
            gameFont = content.Load<SpriteFont>("Spritefonts/gamefont");

            //INITIALZE PLAYERS
            initPlayers();

            //INITIALIZE PARTICLE SYSTEM
            //explosion = new ExplosionEmitter(ScreenManager.Game, 1, 0.2f);
            //ScreenManager.Game.Components.Add(explosion);
            //explosion.active = false;

            //INITIALIZE THE LEVEL
            initlevel(totalPlayers);
            hud = new HUD(ScreenManager.Game, graphics, players, timer, gameFont);
            ScreenManager.Game.Components.Add(hud);
            setPaused(true);

            //INITIALIZE SPAWN MANAGER
            //spawn = new SpawnManager(ScreenManager.Game, this);
            //ScreenManager.Game.Components.Add(spawn);

            //LOAD HUD
           

            //START TIMER
            ScreenManager.Game.ResetElapsedTime();

            Missile m;
            for (int i = 0; i < 50; i++)
            {
                m = new Missile(ScreenManager.Game, Vector3.Zero, Vector3.Zero, new Quaternion(), 0);
                m.active = false;
                missileBuffer.Enqueue(m);
            }
            setPaused(false);
        }

        #endregion


        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!paused)
            {
                updateCamera();
                hud.UpdateHUD(players, paused, timer);
                if (timer!=null) 
                timer.Update(gameTime);
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                //CHECK FOR MISSLE + SHIP COLLISIONS
                checkCollision(gameTime);

                //CHECK FOR END OF GAME
                if (currentPlayers == 1)
                {
                    Player s;
                    int i = 0;

                    while (players[i] == null)
                    {
                        i++;
                    }

                    s = players[i];
                    endRoundDraw();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!paused)
            {
                //CLEAR SCREEN
                ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

                //GRAB THE MAIN SPRITEBATCH
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
           
                //DRAW HUD
                //spriteBatch.Begin();
                //drawHUD(spriteBatch);
                //spriteBatch.End();

                // If the game is transitioning on or off, fade it out to black.
                if (TransitionPosition > 0)
                    ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
            }
        }

        #endregion


        #region Handle Input

        public override void HandleInput(InputState input, GameTime gameTime)
        {
            //MAKES SURE INPUT EXISTS
            if (input == null)
                throw new ArgumentNullException("input");

            //CHECK FOR PAUSE
            if (input.PauseGame)
            {
                setPaused(true);
               
                
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen(graphics));
            }

            //CHECK PLAYER INPUT
            else
            {
                GamePadState currentState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
                InputChecker(currentState, 0, input);
                currentState = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular);
                InputChecker(currentState, 1, input);
                currentState = GamePad.GetState(PlayerIndex.Three, GamePadDeadZone.Circular);
                InputChecker(currentState, 2, input);
                currentState = GamePad.GetState(PlayerIndex.Four, GamePadDeadZone.Circular);
                InputChecker(currentState, 3, input);

               
            }
        }

        public void InputChecker(GamePadState currentState, int i, InputState input)
        {
            if (!paused)
            {
                switch (i)
                {
                    case 0:
                        input.IndexChanger(PlayerIndex.One);
                        break;
                    case 1:
                        input.IndexChanger(PlayerIndex.Two);
                        break;
                    case 2:
                        input.IndexChanger(PlayerIndex.Three);
                        break;
                    case 3:
                        input.IndexChanger(PlayerIndex.Four);
                        break;
                }

                if (currentState.IsConnected)
                {
                    if (players[i] != null)
                    {
                        if (input.LeftThumbStickMovement)
                        {
                            players[i].moveJoystick(currentState.ThumbSticks.Left);
                        }
                        else if (input.LetGoLeftThumbStickMovement)
                        {
                            players[i].stopMoving();
                        }

                        if (input.RightThumbStickRotation)
                        {

                            players[i].rotateJoystick(currentState.ThumbSticks.Right);
                        }

                        if (input.Shoot)
                        {
                            players[i].fire();
                        }

                        if (input.LeftDPad)
                        {
                            players[i].switchWeapon(0);
                        }

                        else if (input.RightDPad)
                        {
                            players[i].switchWeapon(1);
                        }

                        else if (input.UpDPad)
                        {
                            players[i].switchWeapon(2);
                        }

                        if (input.Reload)
                        {
                            players[i].guns[players[i].currentGun].reload();
                        }
                    }
                }
            }
        }

        #endregion


        public override void UnloadContent()
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(GameStateManagementGame.menuMusic);
            content.Unload();
        }


        #region Helper Functions

        public override void initPlayers()
        {
            currentPlayers = totalPlayers;
            for (int i = 0; i < totalPlayers; i++)
            {
                Player temp = new Player(ScreenManager.Game, this, graphics, timer, i);
                players[i] = temp;
                switch (i)
                {
                    case 0:
                        players[i].setMeshName("Player1");
                        if(totalPlayers==2)
                            players[i].SetWorldPosition(-1600, -1600, 0);
                        else if(totalPlayers==3)
                            players[i].SetWorldPosition(-1600, 1600, 0);
                        else if (totalPlayers == 4)
                            players[i].SetWorldPosition(-1600, 1600, 0);
                        break;
                    case 1:
                        players[i].setMeshName("Player2");
                        if (totalPlayers == 2)
                            players[i].SetWorldPosition(1600, 1600, 0);
                        else if (totalPlayers == 3)
                            players[i].SetWorldPosition(1600, 1600, 0);
                        else if (totalPlayers == 4)
                            players[i].SetWorldPosition(1600, 1600, 0);
                        break;
                    case 2:
                        players[i].setMeshName("Player3");
                        if (totalPlayers == 3)
                            players[i].SetWorldPosition(0, -1600, 0);
                        else if (totalPlayers == 4)
                            players[i].SetWorldPosition(-1600, -1600, 0);
                        break;
                    case 3:
                        players[i].setMeshName("Player4");
                        players[i].SetWorldPosition(1600, -1600, 0);
                        break;
                }

                ScreenManager.Game.Components.Add(temp);
                //players[i].lo
                //players[i].Initialize();
            }
        }

        private void updateCamera()
        {
            float lowestX = 32000;
            float biggestX = 0;
            float lowestY = 32000;
            float biggestY = 0;
            float count = 0;
            centerX = 0;
            centerY = 0;

            for (int i = 0; i < totalPlayers; i++)
            {
                if (players[i] != null)
                {
                    if (lowestY > players[i].GetWorldPosition2().Y)
                    {
                        lowestY = players[i].GetWorldPosition2().Y;
                    }
                    if (biggestY < players[i].GetWorldPosition2().Y)
                    {
                        biggestY = players[i].GetWorldPosition2().Y;
                    }
                    if (lowestX > players[i].GetWorldPosition2().X)
                    {
                        lowestX = players[i].GetWorldPosition2().X;
                    }
                    if (biggestX < players[i].GetWorldPosition2().X)
                    {
                        biggestX = players[i].GetWorldPosition2().X;
                    }
                    //centerX += players[i].GetWorldPosition2().X;
                    //centerY += players[i].GetWorldPosition2().Y;
                    centerX = ((biggestX + lowestX) / 2);
                    centerY = ((biggestY + lowestY) / 2);
                    count++;
                }
            }

            //centerX = centerX / count;
            //centerY = centerY / count;
            float x_len;
            if (biggestX > 0 && lowestX > 0) {
                x_len = Math.Abs(biggestX);
            }
            else {
                x_len = Math.Abs(lowestX) + Math.Abs(biggestX);
            }
            float y_len;
            if (biggestY > 0 && lowestY > 0)
            {
                y_len = Math.Abs(biggestY);
            }
            else
            {
                y_len = Math.Abs(lowestY) + Math.Abs(biggestY);
            }

            if (x_len > y_len)
            {
                y_len = (x_len);
            }
            else if (x_len < y_len)
            {
                x_len = (y_len);
            }

            Matrix m2;
            float scale = 0;
            if (currentPlayers == 3)
            {
                m2  = Matrix.CreateOrthographic(x_len + 1000, y_len + 1000, 0f, 20000.0f);
                scale = 1f;
            }
            else if (currentPlayers == 2)
            {
                m2 = Matrix.CreateOrthographic(x_len + 1000, y_len + 1000, 0f, 20000.0f);
                scale = 1f;
            }
             else
            {
                m2 = Matrix.CreateOrthographic(x_len + 1000, y_len + 1000, 0f, 20000.0f);
                scale = 1f;
            }
            
            projectionMatrix = Matrix.Lerp(projectionMatrix, m2, scale);
            //cameraMatrix = Matrix.CreateLookAt(new Vector3(0f, 0f, 2000.0f), new Vector3(centerX, centerY, 0f), Vector3.UnitY);
            cameraMatrix = Matrix.CreateLookAt(new Vector3(centerX, centerY - 500, 2000f), new Vector3(centerX, centerY, 0f), Vector3.UnitY);
        }
        /*
        private void drawHUD(SpriteBatch spriteBatch)
        {
            if (!paused)
            {
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

                    spriteBatch.DrawString(gameFont, "Ammo: " + players[0].guns[players[0].currentGun].clipAmmo + "/" + players[0].guns[players[0].currentGun].ammo, new Vector2(10, 50), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

                    if (!players[0].alive)
                    {
                        explosion.active = true;
                        twoDCoord = graphics.GraphicsDevice.Viewport.Project(Vector3.Zero, projectionMatrix, cameraMatrix, players[0].worldMatrix);
                        explosion.AddParticles(new Vector2(twoDCoord.X, twoDCoord.Y));
                        timer.AddTimer("killP1", .5f, killPlayerOne, false);
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

                    spriteBatch.DrawString(gameFont, "Ammo: " + players[1].guns[players[0].currentGun].clipAmmo + "/" + players[1].guns[players[1].currentGun].ammo, new Vector2(650, 50), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

                    if (!players[1].alive)
                    {
                        explosion.active = true;
                        twoDCoord = graphics.GraphicsDevice.Viewport.Project(Vector3.Zero, projectionMatrix, cameraMatrix, players[1].worldMatrix);
                        explosion.AddParticles(new Vector2(twoDCoord.X, twoDCoord.Y));
                        timer.AddTimer("killP2", .5f, killPlayerTwo, false);
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

                    spriteBatch.DrawString(gameFont, "Ammo: " + players[2].guns[players[2].currentGun].clipAmmo + "/" + players[2].guns[players[2].currentGun].ammo, new Vector2(10, 690), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

                    if (!players[2].alive)
                    {
                        explosion.active = true;
                        twoDCoord = graphics.GraphicsDevice.Viewport.Project(Vector3.Zero, projectionMatrix, cameraMatrix, players[2].worldMatrix);
                        explosion.AddParticles(new Vector2(twoDCoord.X, twoDCoord.Y));
                        timer.AddTimer("killP3", .5f, killPlayerThree, false);

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

                    spriteBatch.DrawString(gameFont, "Ammo: " + players[3].guns[players[3].currentGun].clipAmmo + "/" + players[3].guns[players[3].currentGun].ammo, new Vector2(650, 690), Color.White,
                            0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

                    if (!players[3].alive)
                    {
                        explosion.active = true;
                        twoDCoord = graphics.GraphicsDevice.Viewport.Project(Vector3.Zero, projectionMatrix, cameraMatrix, players[3].worldMatrix);
                        explosion.AddParticles(new Vector2((players[3].Position.X + 1024 / 2) - centerX, (-players[3].Position.Y + 768 / 2) + centerY));
                        timer.AddTimer("killP4", .5f, killPlayerFour, false);

                    }
                }
            }
        }
        */
        public void killPlayer(int i)
        {
            ScreenManager.Game.Components.Remove(players[i]);
            players[i] = null;
            currentPlayers--;
        }

        /*private void killPlayerOne()
        {
            ScreenManager.Game.Components.Remove(players[0]);
            players[0] = null;
            currentPlayers--;
            timer.RemoveTimer("killP1");
        }

        private void killPlayerTwo()
        {
            ScreenManager.Game.Components.Remove(players[1]);
            players[1] = null;
            currentPlayers--;
            timer.RemoveTimer("killP2");
        }

        private void killPlayerThree()
        {
            ScreenManager.Game.Components.Remove(players[2]);
            players[2] = null;
            currentPlayers--;
            timer.RemoveTimer("killP3");
        }

        private void killPlayerFour()
        {
            ScreenManager.Game.Components.Remove(players[3]);
            players[3] = null;
            currentPlayers--;
            timer.RemoveTimer("killP4");
        }*/
        /*
        private void fire(int i)
        {
            
            Player s = players[i];
            if (s == null)
                return;
            if (s.canFire)
            {
                if (s.guns[s.currentGun].clipAmmo > 0)
                {
                    s.canFire = false;
                    s.guns[s.currentGun].clipAmmo--;

                    if (s.currentGun == 0 || s.currentGun == 1)
                    {
                        Missile m = new Missile(ScreenManager.Game, s.Position, s.worldMatrix.Forward, s.Rotation, i);
                        ScreenManager.Game.Components.Add(m);
                        missiles.Add(m);
                    }
                    else
                    {
                        Mine m = new Mine(ScreenManager.Game, s.Position, s.worldMatrix.Forward, s.Rotation, i);
                        ScreenManager.Game.Components.Add(m);
                        mines.Add(m);
                        timer.AddTimer("mineTimer" + i, 2f, new Utils.TimerDelegate(m.Activate), false);
                    }
                    if (s.currentGun == 0)
                    {
                        timer.AddTimer("missileTimer" + i, 0.5f, new Utils.TimerDelegate(s.coolDown), false);
                    }
                    else if (s.currentGun == 1)
                    {
                        timer.AddTimer("missileTimer" + i, 0.05f, new Utils.TimerDelegate(s.coolDown), false);
                    }
                    else
                    {
                        timer.AddTimer("missileTimer"+ i, 1f, new Utils.TimerDelegate(s.coolDown), false);
                    }
                }
            }
            s = null;
        }
        */

        /*public void addMissile(Missile m)
        {
            ScreenManager.Game.Components.Add(m);
            missiles.Add(m);
        }*/

        public void addMine(Mine m)
        {
            ScreenManager.Game.Components.Add(m);
            mines.Add(m);
        }
        public override void deleteObjects()
        {
            for (int i = 0; i < totalPlayers; i++)
            {
                if (players[i] != null)
                {
                    killPlayer(i);
                }
            }

            foreach (Missile m in missiles)
            {
                m.active = false;
                ScreenManager.Game.Components.Remove(m);
                missileBuffer.Enqueue(m);
            }
            foreach (Mine m in mines)
            {
                ScreenManager.Game.Components.Remove(m);
            }
            foreach (Powerup p in powerups)
            {
                ScreenManager.Game.Components.Remove(p);
            }

            missiles.Clear();
            mines.Clear();
            powerups.Clear();
        }

        public override void deleteMissiles() {
            missileBuffer.Clear();
        }
        public override List<Wall> getWalls()
        {
            return walls;
        }

        public override void setPaused(Boolean b)
        {
            paused = b;
            hud.setPaused(paused);
            for (int i = 0; i < totalPlayers; i++)
            {
                if (players[i] != null)
                {
                    players[i].setPaused(paused);
                }
            }
            for (int i = 0; i < walls.Count; i++)
            {
                walls[i].setPaused(paused);
            }
            foreach (Missile m in missiles)
            {
                m.setPaused(paused);
            }
            foreach (Mine m in mines)
            {
                m.setPaused(paused);
            }
            foreach (Powerup p in powerups)
            {
                p.setPaused(paused);
            }
            
        }
        
        public override void deleteHUD()
        {
            ScreenManager.Game.Components.Remove(hud);
            //hud = null;
        }

        public Boolean getPaused()
        {
            return paused;
        }
        /*
        private void respawnShip(Player s, int i)
        {
            s = new Player(ScreenManager.Game, timer, i);
            ScreenManager.Game.Components.Add(s);
            timer.RemoveTimer("respawnTimer");
        }
        */
        private void endRound(Player s, int i)
        {
            setPaused(true);
            int num = i + 1;
            string message = "Player " + num + " won! Play again?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, graphics);

            confirmQuitMessageBox.Accepted += startNewRound;

            ScreenManager.AddScreen(confirmQuitMessageBox);
        }
        
        public void startNewRound(object sender, EventArgs e)
        {
            timer = new Utils.Timer();
            timer.RemoveTimer("roundTimer");
            
            timer.AddTimer("roundTimer", roundTime, new Utils.TimerDelegate(endRoundDraw), false);
            hud.UpdateHUD(players,false,timer);
            
            deleteObjects();
            initPlayers();
            addPowerups();
            setPaused(false);
        }

        public override void deletePowerups()
        {
            for (int i = 0; i < powerups.Count; i++)
            {
                ScreenManager.Game.Components.Remove(powerups[i]);
            }
            powerups.Clear();
        }

        public override void deleteWalls()
        {
            for (int i = 0; i < walls.Count; i++)
            {
                ScreenManager.Game.Components.Remove(walls[i]);
            }
            walls.Clear();
        }

        private void endRoundDraw()
        {
            timer.RemoveTimer("roundTimer");
            timer = null;
            int count = 0;
            setPaused(true);
            List<int> playersInt = new List<int>(); //what?
            for (int i = 0; i < totalPlayers; i++)
            {
                if (players[i] != null)
                {
                    count++;
                    playersInt.Add(i);
                }
            }
            if (count > 1)
            {

                //Implement end round (display message and wait for a few seconds to start new round and restart with new round code
                //startNewRound()
                String numbers = "";
                for (int i = 0; i < playersInt.Count; i++)
                {
                    int num = playersInt[i] + 1;
                    numbers += num + " ";
                    if (i + 2 == playersInt.Count)
                    {
                        numbers += "and ";
                    }
                    else if (i + 2 != playersInt.Count && i + 1 != playersInt.Count)
                    {
                        numbers += ", ";
                    }
                }
                
                //Implement end round (display message and wait for a few seconds to start new round and restart with new round code
                //Increment player score/round wins
                string message = "Players " + numbers + " draw! Play again?";

                MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, graphics);

                confirmQuitMessageBox.Accepted += startNewRound;

                ScreenManager.AddScreen(confirmQuitMessageBox);
            }
            else
            {
                Player temp;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] != null)
                    {
                        temp = players[i];
                        temp.setPaused(true);
                        endRound(temp, i);
                        break;
                    }
                }
            }
        }

        public void checkCollision(GameTime gameTime)
        {
            missilesToRemove.Clear();
            powerupsToRemove.Clear();
            minesToRemove.Clear();

            for (int i = 0; i < totalPlayers; i++)
            {
                if (players[i] != null)
                {
                    players[i].setCamera(projectionMatrix, cameraMatrix);

                    foreach (Missile m in missiles)
                    {
                        if (m.looped)
                        {
                            missilesToRemove.Add(m);
                        }
                        if (i < 4)
                        {
                            m.worldBounds.Radius /= 1.5f;

                            if (players[i].worldBounds.Intersects(m.worldBounds) && m.playerIndex != i)
                            {
                                //players[i].setCamera(projectionMatrix, cameraMatrix);
                                players[i].health -= m.damage;
                                players[i].Hit = true;
                                missilesToRemove.Add(m);
                            }
                        }
                    }

                    foreach (Mine mi in mines)
                    {
                        if (mi.active == true)
                        {
                            if (players[i].worldBounds.Intersects(mi.worldBounds))
                            {
                                //players[i].setCamera(projectionMatrix, cameraMatrix);
                                players[i].health -= mi.damage;
                                players[i].Hit = true;
                                mi.explode();
                                mi.drawMine = false;
                                minesToRemove.Add(mi);
                            }
                        }
                        
                    }

                    if (players[i].health <= 0)
                    {
                        players[i].alive = false;
                    }

                    foreach (Powerup p in powerups)
                    {
                        if (players[i].worldBounds.Intersects(p.worldBounds))
                        {
                            players[i].guns[1].addAmmo(p.getType());
                            players[i].guns[2].addAmmo(p.getType());
                            powerupsToRemove.Add(p);
                        }
                    }

                    foreach (Player player in players)
                    {
                        if (player != null)
                        {
                            if (players[i].worldBounds.Intersects(player.worldBounds) && player != players[i])
                            {
                                Vector3 temp;
                                temp = players[i].GetOldWorldPosition();
                                players[i].SetWorldPosition(temp.X, temp.Y, temp.Z);
                            }
                        }
                    }
                }
            }

            for(int j=0; j < walls.Count; j++)
            {
                Wall w = walls[j];
                BoundingBox box = w.boundingBox;
                foreach (Missile m in missiles)
                {
                    if (walls[j].boundingBox.Intersects(m.worldBounds))
                    {
                        missilesToRemove.Add(m);
                    }
                }

                for (int i = 0; i < totalPlayers; i++)
                {
                    if (players[i] != null)
                    {
                        if (players[i].worldBounds.Intersects(w.boundingBox))
                        {
                            if (w.Rotation.W <= 0.6f)
                            {
                                if (players[i].Position.X >= w.Position.X)
                                {
                                    players[i].Position = new Vector3(players[i].Position.X + players[i].worldBounds.Radius - (players[i].Position.X - w.Position.X) + 15,
                                         players[i].Position.Y, players[i].Position.Z);
                                }
                                else
                                {
                                    players[i].Position = new Vector3(players[i].Position.X - players[i].worldBounds.Radius + (w.Position.X - players[i].Position.X) - 15,
                                         players[i].Position.Y, players[i].Position.Z);
                                }
                            }
                            if (w.Rotation.W > 0.6f)
                            {
                                if (players[i].Position.Y >= w.Position.Y)
                                {
                                    players[i].Position = new Vector3(players[i].Position.X, players[i].Position.Y + players[i].worldBounds.Radius - (players[i].Position.Y - w.Position.Y) + 15,
                                         players[i].Position.Z);
                                    //players[i].canUp = false;
                                }
                                else
                                {
                                    players[i].Position = new Vector3(players[i].Position.X, players[i].Position.Y - players[i].worldBounds.Radius + (w.Position.Y - players[i].Position.Y) - 15,
                                         players[i].Position.Z);
                                }
                            }
                        }
                    }
                }
            }

            foreach (Missile m in missilesToRemove)
            {
                m.active = false;
                missiles.Remove(m);
                ScreenManager.Game.Components.Remove(m);
                missileBuffer.Enqueue(m);
            }

            foreach (Powerup p in powerupsToRemove)
            {
                ScreenManager.Game.Components.Remove(p);
                powerups.Remove(p);
            }

            foreach (Mine m in minesToRemove)
            {
                ScreenManager.Game.Components.Remove(m);
                mines.Remove(m);
            }
        }

        public void addPowerups()
        {
            switch (totalPlayers)
            {
                case 2:
                    //POWERUPS
                    powerup = new Powerup(ScreenManager.Game, 400, -400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -400, 400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 1400, -1400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1400, 1400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);
                    break;

                case 3:
                    powerup = new Powerup(ScreenManager.Game, 0, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 0, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                     powerup = new Powerup(ScreenManager.Game, -1600, 200, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 1600, 200, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);
                    break;

                case 4:
                    powerup = new Powerup(ScreenManager.Game, 0, 0, 0, 1);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, 700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, -700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                     powerup = new Powerup(ScreenManager.Game, 1600, 700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 1600, -700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 700, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -700, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 700, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -700, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);
                    break;
            }
        }

        public void initlevel(int totalPlayers)
        {
           
            //floor = new Floor(ScreenManager.Game);
            //ScreenManager.Game.Components.Add(floor);
            //COMMENT OUT IF YOU WANT TO INCREASE LOAD TIMES

            //Border Walls
            //Top
            int z = 0;

            walls.Clear();
            switch (totalPlayers)
            {
                case 1:
                    players[0].SetWorldPosition(0, 0, 0);
                    //Border Walls
                    //Top
                    wall = new Wall(ScreenManager.Game, -1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //bottom

                    wall = new Wall(ScreenManager.Game, -1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //left

                    wall = new Wall(ScreenManager.Game, -1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //right


                    wall = new Wall(ScreenManager.Game, 1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    break;
                case 2:
                    //TWO PLAYERS


                    //Border Walls
                    //Top
                    wall = new Wall(ScreenManager.Game, -1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //bottom

                    wall = new Wall(ScreenManager.Game, -1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //left

                    wall = new Wall(ScreenManager.Game, -1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //right


                    wall = new Wall(ScreenManager.Game, 1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Top Left Corner
                    wall = new Wall(ScreenManager.Game, -100, 400, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -100, 1600, z, 2);                    
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -400, 100, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1600, 100, z, 2);                    
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Bottom Right Corner
                    wall = new Wall(ScreenManager.Game, 100, -400, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 100, -1600, z, 2);                    
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 400, -100, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1600, -100, z, 2);                    
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //Top Right
                    wall = new Wall(ScreenManager.Game, 1000, 700, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 700, 1000, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Bottom Left
                    wall = new Wall(ScreenManager.Game, -1000, -700, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -700, -1000, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Top Left
                    wall = new Wall(ScreenManager.Game, 1000, -1000, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1000, -1000, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Bottom Right
                    wall = new Wall(ScreenManager.Game, -1000, 1000, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1000, 1000, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //POWERUPS
                    powerup = new Powerup(ScreenManager.Game, 400, -400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -400, 400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 1400, -1400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1400, 1400, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);
                    break;

                case 3:
                    //THREE PLAYERS

                    //Border Walls
                    //Top
                    wall = new Wall(ScreenManager.Game, -1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //bottom

                    wall = new Wall(ScreenManager.Game, -1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //left

                    wall = new Wall(ScreenManager.Game, -1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //right


                    wall = new Wall(ScreenManager.Game, 1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //All New Additions
                    wall = new Wall(ScreenManager.Game, 300, -1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    
                    wall = new Wall(ScreenManager.Game, -300, -1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 300, 1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    
                    wall = new Wall(ScreenManager.Game, -300, 1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 0, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 0, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1600, 0, z, 2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1600, 0, z, 2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -500, 0, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 500, 0, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    wall = new Wall(ScreenManager.Game, 1100, 900, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1100, 900, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1100, -900, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1100, -900, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //POWERUPS
                    powerup = new Powerup(ScreenManager.Game, 0, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 0, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                     powerup = new Powerup(ScreenManager.Game, -1600, 200, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 1600, 200, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    break;
              
                
                
                
                
                case 4:
                    //Four PLayers
                    //Border Walls
                    //Top
                    wall = new Wall(ScreenManager.Game, -1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, 1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //bottom

                    wall = new Wall(ScreenManager.Game, -1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 300, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 900, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1500, -1800, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //left

                    wall = new Wall(ScreenManager.Game, -1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, -1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);


                    //right


                    wall = new Wall(ScreenManager.Game, 1800, -1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, -300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 300, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 900, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);
                    wall = new Wall(ScreenManager.Game, 1800, 1500, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);



                    //Corner Spawns
                    //Top Left
                    wall = new Wall(ScreenManager.Game, -1600, 900, z, 2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -900, 1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1300, 1400, z, 1);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1400, 1300, z, 1);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Top Right
                    wall = new Wall(ScreenManager.Game, 1600, 900, z, 2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 900, 1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1300, 1400, z, 1);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1400, 1300, z, 1);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Bottom Right
                    wall = new Wall(ScreenManager.Game, 1600, -900, z, 2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 900, -1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1300, -1400, z, 1);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1400, -1300, z, 1);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Bottom Left
                    wall = new Wall(ScreenManager.Game, -1600, -900, z, 2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -900, -1600, z, 2);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1300, -1400, z, 1);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1400, -1300, z, 1);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //Center
                    wall = new Wall(ScreenManager.Game, 500, 0, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -500, 0, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 0, 500, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 0, -500, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 0, -1100, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 0, 1100, z, 3);
                    wall.createRotation(new Vector3(0f, 0f, 1f), MathHelper.PiOver2);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, 1100, 0, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    wall = new Wall(ScreenManager.Game, -1100, 0, z, 3);
                    walls.Add(wall);
                    ScreenManager.Game.Components.Add(wall);

                    //PowerUps
                    powerup = new Powerup(ScreenManager.Game, 0, 0, z, 1);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, 700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -1600, -700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                     powerup = new Powerup(ScreenManager.Game, 1600, 700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 1600, -700, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, 700, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -700, -1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);
                   
                    
                    powerup = new Powerup(ScreenManager.Game, 700, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);

                    powerup = new Powerup(ScreenManager.Game, -700, 1600, 0, 2);
                    powerups.Add(powerup);
                    ScreenManager.Game.Components.Add(powerup);
                    break;                  
            }
        }
        #endregion

        internal void fire(Vector3 Position, Vector3 vector3, Quaternion Rotation, int index)
        {
            mTemp = missileBuffer.Dequeue();
            mTemp.active = true;
            mTemp.setMissile(Position, vector3, Rotation, index);
            ScreenManager.Game.Components.Add(mTemp);
            missiles.Add(mTemp);
        }
    }
}
