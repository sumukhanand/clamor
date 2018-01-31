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
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry roundTimeOption;

        enum RoundTimes
        {
            IgnoreThis,
            One,
            Two,
            Three,
            Four
        }

        static RoundTimes currentRoundTime = RoundTimes.One;

        GraphicsDeviceManager graphics;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen(GraphicsDeviceManager g)
            : base("")
        {
            // Create our menu entries.
            roundTimeOption = new MenuEntry(string.Empty, "", false);

            SetMenuEntryText();

            MenuEntry backMenuEntry = new MenuEntry("Back to Main Menu", "back", true);

            // Hook up menu event handlers.
            roundTimeOption.Selected += RoundTimeSelected;
            backMenuEntry.Selected += back;

            // Add entries to the menu.
            MenuEntries.Add(roundTimeOption);
            MenuEntries.Add(backMenuEntry);

            graphics = g;
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            roundTimeOption.Text = "Round Time (Minutes): " + currentRoundTime;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void RoundTimeSelected(object sender, EventArgs e)
        {
            currentRoundTime++;

            if (currentRoundTime > RoundTimes.Four)
                currentRoundTime = (RoundTimes)1;

            SetMenuEntryText();
        }

        void back(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            MainMenuScreen mainScreen = new MainMenuScreen(graphics, (int)currentRoundTime*60);
            BackgroundScreen backgroundScreen = new BackgroundScreen("main_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(mainScreen);
        }

        protected override void OnCancel()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            MainMenuScreen mainScreen = new MainMenuScreen(graphics, (int)currentRoundTime*60);
            BackgroundScreen backgroundScreen = new BackgroundScreen("main_background");
            ScreenManager.AddScreen(backgroundScreen);
            ScreenManager.AddScreen(mainScreen);
        }

        #endregion
    }
}
