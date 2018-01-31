#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        ContentManager content;
        Texture2D image;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input, GameTime gameTime)
        {
            // Move to the previous menu entry?
            if (input.MenuUp || input.MenuLeft)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.MenuDown || input.MenuRight)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu?
            if (input.MenuSelect)
            {
                OnSelectEntry(selectedEntry);
            }
            else if (input.MenuCancel)
            {
                OnCancel();
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Vector2 position = new Vector2(150, 300);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            //spriteBatch.Begin();
            int count = 1;
            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                //count += 1;
                MenuEntry menuEntry = menuEntries[i];
                if (menuEntry.textb)
                {
                    if (count == 1)
                    {
                        position.X = 100;
                        if ((i + 1) == menuEntries.Count || menuEntry.Text.Contains("Back"))
                        {
                            position.Y = 625;
                            position.X = 350;
                        }
                    }
                    else if (count == 2)
                    {
                        position.X = 600;
                    }
                    bool isSelected = IsActive && (i == selectedEntry);
                    if (content == null)
                        content = new ContentManager(ScreenManager.Game.Services, "Content");
                    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                    this.image = content.Load<Texture2D>("Images/" + menuEntry.s_image);
                    menuEntry.Draw(this, position, isSelected, gameTime, spriteBatch, viewport, content);
                    //count++;

                    if (count == 1)
                    {
                        position.Y += 0;
                        count = 2;
                    }
                    else if (count == 2)
                    {
                        position.Y = +500;
                        count = 1;
                    }
                }
                else
                {
                    position.X = 100;
                    if (menuEntry.Text.Contains("Minutes"))
                    {
                        position.X = 250;
                    }
                    bool isSelected = IsActive && (i == selectedEntry);
                    if (content == null)
                        content = new ContentManager(ScreenManager.Game.Services, "Content");
                    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                    //this.image = content.Load<Texture2D>("Images/" + menuEntry.s_image);
                    menuEntry.Draw(this, position, isSelected, gameTime, spriteBatch, viewport, content);
                    position.Y += 50;
                }

            }

            // Draw the menu title.
            Vector2 titlePosition = new Vector2(512, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;
            spriteBatch.Begin();
            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }




        #endregion
    }
}
