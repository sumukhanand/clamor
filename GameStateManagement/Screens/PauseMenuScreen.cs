#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        GraphicsDeviceManager graphics;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(GraphicsDeviceManager g)
            : base("")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game", "main_play", true);
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game", "main_quit", true);

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

            graphics = g;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message, graphics);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            //GameplayScreen.players.clear();
            //Console.WriteLine("" + sender.GetType());
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                if (screen.name.Equals("game"))
                {
                    screen.deleteHUD();
                    screen.deleteObjects();
                    screen.setPaused(false);
                    screen.deleteWalls();
                    screen.initlevel();
                }
            }
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen("main_background"),
                                                     new MainMenuScreen(graphics,0));
        }

        protected override void OnCancel()
        {   
            
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                if (screen.name.Equals("game"))
                {
                    screen.setPaused(false);
                    ExitScreen();
                    break;
                }
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }


        #endregion
    }
}
