#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
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
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class InfoMenuScreen : MenuScreen
    {
        #region Initialization

        public GraphicsDeviceManager graphics;

        /// <summary>
        /// Constructor.
        /// </summary>
        public InfoMenuScreen(GraphicsDeviceManager g)
            : base("")
        {

            MenuEntry backMenuEntry = new MenuEntry("Back to Main Menu", "back", true);

            // Hook up menu event handlers.
            backMenuEntry.Selected += back;

            // Add entries to the menu.
            MenuEntries.Add(backMenuEntry);

            graphics = g;
        }

        void back(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            MainMenuScreen mainScreen = new MainMenuScreen(graphics, 0);
            BackgroundScreen backgroundScreen = new BackgroundScreen("main_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(mainScreen);
        }

        protected override void OnCancel()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            MainMenuScreen mainScreen = new MainMenuScreen(graphics, 0);
            BackgroundScreen backgroundScreen = new BackgroundScreen("main_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(mainScreen);
        }

        #endregion
    }
}
