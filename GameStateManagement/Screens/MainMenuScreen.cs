#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        private GraphicsDeviceManager graphics;
        static int roundTime = 0;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(GraphicsDeviceManager g, int time)
            : base("")
        {
            roundTime = time;
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(GameStateManagementGame.menuMusic);
            }

            // Create our menu entries.
            MenuEntry playMenuEntry = new MenuEntry("Play", "main_play", true);
            MenuEntry optionsMenuEntry = new MenuEntry("Options", "main_options", true);
            MenuEntry infoMenuEntry = new MenuEntry("Info", "main_info", true);
            MenuEntry quitMenuEntry = new MenuEntry("Quit", "main_quit", true);

            // Hook up menu event handlers.
            playMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            infoMenuEntry.Selected += InfoMenuEntrySelected;
            quitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(infoMenuEntry);
            MenuEntries.Add(quitMenuEntry);

            graphics = g;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            PlayersScreen playersScreen = new PlayersScreen(graphics, roundTime);
            BackgroundScreen backgroundScreen = new BackgroundScreen("players_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(playersScreen);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            OptionsMenuScreen optionsScreen = new OptionsMenuScreen(graphics);
            BackgroundScreen backgroundScreen = new BackgroundScreen("options_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(optionsScreen);
        }

        void InfoMenuEntrySelected(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            InfoMenuScreen infoScreen = new InfoMenuScreen(graphics);
            BackgroundScreen backgroundScreen = new BackgroundScreen("info_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(infoScreen);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to quit the game?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, graphics);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
