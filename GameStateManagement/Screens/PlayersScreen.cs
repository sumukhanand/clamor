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
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class PlayersScreen : MenuScreen
    {
        #region Initialization

        private GraphicsDeviceManager graphics;
        static int roundTime = 60;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public PlayersScreen(GraphicsDeviceManager g, int time)
            : base("")
        {
            if (time != 0) {
                roundTime = time;
            }
            // Create our menu entries.
            MenuEntry twoMenuEntry = new MenuEntry("2", "players_two", true);
            MenuEntry threeMenuEntry = new MenuEntry("3", "players_three", true);
            MenuEntry fourMenuEntry = new MenuEntry("4", "players_four", true);
            MenuEntry backMenuEntry = new MenuEntry("Back to Main Menu", "back", true);

            // Hook up menu event handlers.
            twoMenuEntry.Selected += twoPlayers;
            threeMenuEntry.Selected += threePlayers;
            fourMenuEntry.Selected += fourPlayers;
            backMenuEntry.Selected += back;

            // Add entries to the menu.
            MenuEntries.Add(twoMenuEntry);
            MenuEntries.Add(threeMenuEntry);
            MenuEntries.Add(fourMenuEntry);
            MenuEntries.Add(backMenuEntry);

            graphics = g;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>

        void twoPlayers(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(2, roundTime, graphics));
        }

        void threePlayers(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(3, roundTime, graphics));
        }

        void fourPlayers(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(4, roundTime, graphics));
        }

        void back(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            MainMenuScreen mainScreen = new MainMenuScreen(graphics, roundTime);
            BackgroundScreen backgroundScreen = new BackgroundScreen("main_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(mainScreen);
        }

        protected override void OnCancel()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            MainMenuScreen mainScreen = new MainMenuScreen(graphics, roundTime);
            BackgroundScreen backgroundScreen = new BackgroundScreen("main_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(mainScreen);
        }

        #endregion
    }
}
