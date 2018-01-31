#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class GameStateManagementGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        public GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        public static Song menuMusic;
        public static Song gameplayMusic;

        #endregion
        public int time = 60;
        #region Properties
        public float fTargetMsPerFrame
        {
            get { return (float)TargetElapsedTime.Milliseconds; }
            set
            {
                if (value > 1.0f)
                    TargetElapsedTime = System.TimeSpan.FromMilliseconds(value);
                else
                    TargetElapsedTime = System.TimeSpan.FromMilliseconds(1.0f);
            }
        }
        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public GameStateManagementGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            menuMusic = Content.Load<Song>("Music/menu");
            gameplayMusic = Content.Load<Song>("Music/gameplay");
            MediaPlayer.IsRepeating = true;

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen("main_background"));
            screenManager.AddScreen(new MainMenuScreen(graphics, 0));

            // For testing purposes, let's disable fixed time step and vsync.
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
        }

        public GraphicsDeviceManager getGDM()
        {
            return graphics;
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }




        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (GameStateManagementGame game = new GameStateManagementGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
