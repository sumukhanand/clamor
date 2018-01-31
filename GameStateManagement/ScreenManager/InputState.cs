#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;
        public PlayerIndex playerIndex = PlayerIndex.One;
        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
        }


        #endregion

        #region Properties


        /// <summary>
        /// Checks for a "menu up" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewButtonPress(Buttons.LeftThumbstickUp);
            }
        }

        public bool MenuLeft
        {
            get
            {
                return IsNewButtonPress(Buttons.LeftThumbstickLeft);
            }
        }

        public bool MenuRight
        {
            get
            {
                return IsNewButtonPress(Buttons.LeftThumbstickRight);
            }
        }


        /// <summary>
        /// Checks for a "menu down" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewButtonPress(Buttons.LeftThumbstickDown);
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewButtonPress(Buttons.A);
            }
        }


        /// <summary>
        /// Checks for a "menu cancel" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewButtonPress(Buttons.B);
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewButtonPress(Buttons.Start);
            }
        }

        //public bool ShipFire
        //{
        //    get
        //    {
        //        return IsNewKeyPress(Keys.Space) ||
        //               IsKeyHeld(Keys.Space) ||
        //               IsNewButtonPress(Buttons.RightTrigger);
        //    }
        //}

        //public bool checkKeyPress(Keys k)
        //{
        //    return IsNewKeyPress(k) ||
        //               IsKeyHeld(k);
        //}

        //public bool ShipTurnLeft
        //{
        //    get
        //    {
        //        return IsNewKeyPress(Keys.A) ||
        //               IsKeyHeld(Keys.A);
        //    }
        //}

        //public bool ShipTurnRight
        //{
        //    get
        //    {
        //        return IsNewKeyPress(Keys.D) ||
        //               IsKeyHeld(Keys.D);
        //    }
        //}

        //public bool ShipMoveForward
        //{
        //    get
        //    {
        //        return IsNewKeyPress(Keys.W) ||
        //               IsKeyHeld(Keys.W);
        //    }
        //}

        //public bool ShipLetGo
        //{
        //    get
        //    {
        //        return IsNewKeyUp(Keys.W);
        //    }
        //}

        //public bool ShipMoveBackward
        //{
        //    get
        //    {
        //        return IsNewKeyPress(Keys.S) ||
        //               IsKeyHeld(Keys.S);
        //    }
        //}

        //CONTROLLER CONTROLS
        public bool LeftThumbStickMovement
        {
            get
            {

                return IsNewButtonPress(Buttons.LeftThumbstickLeft, playerIndex) ||
                    IsButtonHeld(Buttons.LeftThumbstickLeft, playerIndex) ||
                        IsNewButtonPress(Buttons.LeftThumbstickRight, playerIndex) ||
                    IsButtonHeld(Buttons.LeftThumbstickRight, playerIndex) ||
                        IsNewButtonPress(Buttons.LeftThumbstickUp, playerIndex) ||
                    IsButtonHeld(Buttons.LeftThumbstickUp, playerIndex) ||
                        IsNewButtonPress(Buttons.LeftThumbstickDown, playerIndex) ||
                    IsButtonHeld(Buttons.LeftThumbstickDown, playerIndex);
            }

        }
        public bool LetGoLeftThumbStickMovement
        {
            get
            {
                return IsNewButtonUp(Buttons.LeftThumbstickLeft, playerIndex) ||
                         IsNewButtonUp(Buttons.LeftThumbstickRight, playerIndex) ||
                         IsNewButtonUp(Buttons.LeftThumbstickUp, playerIndex) ||
                         IsNewButtonUp(Buttons.LeftThumbstickDown, playerIndex);
            }
        }
        public bool RightThumbStickRotation
        {
            get
            {
                return IsNewButtonPress(Buttons.RightThumbstickLeft, playerIndex) ||
                        IsButtonHeld(Buttons.RightThumbstickLeft, playerIndex) ||
                        IsNewButtonPress(Buttons.RightThumbstickRight, playerIndex) ||
                        IsButtonHeld(Buttons.RightThumbstickRight, playerIndex) ||
                        IsNewButtonPress(Buttons.RightThumbstickUp, playerIndex) ||
                        IsButtonHeld(Buttons.RightThumbstickUp, playerIndex) ||
                        IsNewButtonPress(Buttons.RightThumbstickDown, playerIndex) ||
                        IsButtonHeld(Buttons.RightThumbstickDown, playerIndex);
            }

        }

        public bool Shoot
        {
            get
            {
                return IsButtonHeld(Buttons.RightTrigger, playerIndex) ||
                        IsNewButtonPress(Buttons.RightTrigger, playerIndex);
            }
        }

        public bool Reload
        {
            get
            {
                return IsNewButtonPress(Buttons.X, playerIndex);
            }
        }

        public bool LeftDPad
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadLeft, playerIndex);
            }
        }

        public bool RightDPad
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadRight, playerIndex);
            }
        }

        public bool UpDPad
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadUp, playerIndex);
            }
        }

        public bool Action
        {
            get
            {
                return IsNewButtonPress(Buttons.A, playerIndex);
            }
        }


        #endregion

        #region Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyPress(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }

        public bool IsNewKeyUp(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyUp(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }

        public bool IsKeyHeld(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyHeld(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }

        public bool IsButtonHeld(Buttons but)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsButtonHeld(but, (PlayerIndex)i))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyUp(key));
        }


        public bool IsKeyHeld(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(key));
        }

        public bool IsButtonHeld(Buttons but, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(but) &&
                    LastGamePadStates[(int)playerIndex].IsButtonDown(but));
        }

        public bool IsNewKeyUp(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyUp(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(key));
        }

        public bool IsNewButtonUp(Buttons but, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonUp(but) &&
                    LastGamePadStates[(int)playerIndex].IsButtonDown(but));
        }
        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewButtonPress(button, (PlayerIndex)i))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonUp(button));
        }

        /// <summary>
        /// Checks for a "menu select" input action from the specified player.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, playerIndex) ||
                   IsNewKeyPress(Keys.Enter, playerIndex) ||
                   IsNewButtonPress(Buttons.A, playerIndex) ||
                   IsNewButtonPress(Buttons.Start, playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action from the specified player.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, playerIndex) ||
                   IsNewButtonPress(Buttons.B, playerIndex) ||
                   IsNewButtonPress(Buttons.Back, playerIndex);
        }

        public void IndexChanger(PlayerIndex plx)
        {
            playerIndex = plx;
        }

        #endregion
    }
}
