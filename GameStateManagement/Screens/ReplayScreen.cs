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
    class ReplayScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ReplayScreen(int i)
            : base("")
        {
            //string message = "Player " + i + 1 + " won! Play again?";
            const string message = "Are you sure you want to quit this game?";
            MessageBoxScreen confirmQuitMessageBox2 = new MessageBoxScreen(message);

            confirmQuitMessageBox2.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox2);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            //GameplayScreen.players.clear();
            Console.WriteLine("" + sender.GetType());
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                if (screen.name.Equals("game"))
                {
                    //screen.startNewRound();
                }
            }
            ExitScreen();
        }

        protected override void OnCancel()
        {   
            ExitScreen();
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                if (screen.name.Equals("game"))
                {
                    screen.setPaused(false);
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
