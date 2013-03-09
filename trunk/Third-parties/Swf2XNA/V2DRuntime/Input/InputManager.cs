﻿#region File Description
//-----------------------------------------------------------------------------
// InputManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using V2DRuntime.Enums;
#endregion

namespace DDW.Input // these are ms supplied classes (and they are obtuse), namespace changed to fit
{
    public class InputManager
    {
		public PlayerIndex PlayerIndex;

		public GamePadState GamePadState;
		public KeyboardState currentKeyboardState;
		public bool IsActiveController = false;
        public static KeyboardState EmptyKeyboardState = new KeyboardState();

        public PlayerJoinState PlayerJoinState = PlayerJoinState.NotJoined;

		public Buttons Releases;

        /// <summary>
        /// The last "real time" that new input was received. Slightly late button
        /// presses will not update this time; they are merged with previous input.
        /// </summary>
        public TimeSpan LastInputTime { get; private set; }

        /// <summary>
        /// The current sequence of pressed buttons.
        /// </summary>
        public List<Buttons> Buffer;

        
        /// <summary>
        /// This is how long to wait for input before all input data is expired.
        /// This prevents the player from performing half of a move, waiting, then
        /// performing the rest of the move after they forgot about the first half.
        /// </summary>
        public readonly TimeSpan BufferTimeOut = TimeSpan.FromMilliseconds(500);

        
        /// <summary>
        /// This is the size of the "merge window" for combining button presses that
        /// occur at almsot the same time.
        /// If it is too small, players will find it difficult to perform moves which
        /// require pressing several buttons simultaneously.
        /// If it is too large, players will find it difficult to perform moves which
        /// require pressing several buttons in sequence.
        /// </summary>
        public readonly TimeSpan MergeInputTime = TimeSpan.FromMilliseconds(100);


        /// <summary>
        /// Provides the map of non-direction game pad buttons to keyboard keys.
        /// </summary>
        internal static readonly Dictionary<Buttons, Keys> NonDirectionButtons =
            new Dictionary<Buttons, Keys>
            {
                { Buttons.A, Keys.A },
                { Buttons.B, Keys.B },
                { Buttons.X, Keys.X },
                { Buttons.Y, Keys.Y },
                
                { Buttons.Start, Keys.Enter },
                { Buttons.Back, Keys.None},
                { Buttons.LeftShoulder, Keys.None},
                { Buttons.LeftTrigger, Keys.None},
                { Buttons.LeftStick, Keys.None},
                { Buttons.RightShoulder, Keys.None},
                { Buttons.RightTrigger, Keys.None},
                { Buttons.RightStick, Keys.None},
            };


        public InputManager(PlayerIndex playerIndex, int bufferSize)
        {
            PlayerIndex = playerIndex;
            Buffer = new List<Buttons>(bufferSize);
        }

        /// <summary>
        /// Gets the latest input and uses it to update the input history buffer.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Get latest input state.
            GamePadState lastGamePadState = GamePadState;
            GamePadState = GamePad.GetState(PlayerIndex);
            KeyboardState lastKeyboardState = EmptyKeyboardState;
#if !XBOX
            // keyboard only happens from one player		
			if (this.IsActiveController)//PlayerIndex == PlayerIndex.One)
			{
				lastKeyboardState = currentKeyboardState;
				currentKeyboardState = Keyboard.GetState(PlayerIndex);
			}
			else
			{
				lastKeyboardState = EmptyKeyboardState;
				currentKeyboardState = EmptyKeyboardState;
			}
#endif            

            // Expire old input.
            TimeSpan time = gameTime.TotalGameTime;
            TimeSpan timeSinceLast = time - LastInputTime;
            if (timeSinceLast > BufferTimeOut)
            {
                Buffer.Clear();
            }

			// Get all of the non-direction buttons pressed.
			Buttons buttons = 0;
			Releases = 0;
            foreach (var buttonAndKey in NonDirectionButtons)
            {
                Buttons button = buttonAndKey.Key;
                Keys key = buttonAndKey.Value;

                // Check the game pad and keyboard for presses.
				if ( lastGamePadState.IsButtonUp(button) && GamePadState.IsButtonDown(button) )
				{
					buttons |= button;
                }
#if !XBOX
                else if( IsActiveController && (lastKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key)) )
				{
					buttons |= button;
				}
#endif
				if ( lastGamePadState.IsButtonDown(button) && GamePadState.IsButtonUp(button) )
				{
					Releases |= button;
                }
#if !XBOX
                else if( IsActiveController && (lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key)) )
				{
					Releases |= button;
				}
#endif
            }

            // It is very hard to press two buttons on exactly the same frame.
            // If they are close enough, consider them pressed at the same time.
            bool mergeInput = (Buffer.Count > 0 && timeSinceLast < MergeInputTime);

            // If there is a new direction,
            var direction = Direction.FromInput(GamePadState, currentKeyboardState);
            if (Direction.FromInput(lastGamePadState, lastKeyboardState) != direction)
            {
                // combine the direction with the buttons.
                buttons |= direction;

                // Don't merge two different directions. This avoids having impossible
                // directions such as Left+Up+Right. This also has the side effect that
                // the direction needs to be pressed at the same time or slightly before
                // the buttons for merging to work.
                mergeInput = false;
            }

            // If there was any new input on this update, add it to the buffer.
            if (buttons != 0)
            {
                if (mergeInput)
                {
                    // Use a bitwise-or to merge with the previous input.
                    // LastInputTime isn't updated to prevent extending the merge window.
                    Buffer[Buffer.Count - 1] = Buffer[Buffer.Count - 1] | buttons;                    
                }
                else
                {
                    // Append this input to the buffer, expiring old input if necessary.
                    if (Buffer.Count == Buffer.Capacity)
                    {
                        Buffer.RemoveAt(0);
                    }
                    Buffer.Add(buttons);

                    // Record this the time of this input to begin the merge window.
                    LastInputTime = time;
                }
            }
        }

        /// <summary>
        /// Determines if a move matches the current input history. Unless the move is
        /// a sub-move, the history is "consumed" to prevent it from matching twice.
        /// </summary>
        /// <returns>True if the move matches the input history.</returns>
        public bool Matches(Move move)
        {
            // If the move is longer than the buffer, it can't possibly match.
            if (Buffer.Count < move.Sequence.Length)
                return false;

            // Loop backwards to match against the most recent input.
            for (int i = 1; i <= move.Sequence.Length; ++i)
            {
                if (Buffer[Buffer.Count - i] != move.Sequence[move.Sequence.Length - i])
                {
                    return false;
                }
            }

            // Rnless this move is a component of a larger sequence,
            if (!move.IsSubMove)
            {
                // consume the used inputs.
                Buffer.Clear();
            }

            return true;
        }

        private static int maxLocalGamers = 4;
        private static InputManager[] inputManagers;
        public static InputManager[] Managers
        {
            get
            {
                if (inputManagers == null)
                {
                    inputManagers = new InputManager[maxLocalGamers];

                    for (int i = 0; i < maxLocalGamers; i++)
                    {
                        if (GamePad.GetState((PlayerIndex)i).IsConnected)
                        {
                            AddInputManager(i);
                        }
                        else
                        {
                            inputManagers[i] = null;
                        }
                    }

                }
                return inputManagers;
            }
        }
        public static void ClearAll()
        {
            for (int i = 0; i < inputManagers.Length; i++)
            {
                if (inputManagers[i] != null)
                {
                    inputManagers[i].Buffer.Clear();
                }
            }
        }

        public static void Update()
        {
            for (int i = 0; i < maxLocalGamers; i++)
            {
                if (inputManagers[i] == null && GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    AddInputManager(i);
                }
                if (inputManagers[i] != null && !GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    inputManagers[i] = null;
                }
            }
        }

        public static void AddInputManager(int playerIndex)
        {
            inputManagers[playerIndex] = new InputManager((PlayerIndex)playerIndex, 2);//, moveList.LongestMoveLength);
        }
    }
}
