using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame
{
    class InputProcessor
    {
        KeyboardState oldState;
        KeyboardState newState;

        public InputProcessor()
        {
            oldState = Keyboard.GetState();
            newState = Keyboard.GetState();
        }

        public void update()
        {
            oldState = newState;
            newState = Keyboard.GetState();
        }

        public bool isKeyReleased(Keys key)
        {
            return newState.IsKeyUp(key) && !oldState.IsKeyUp(key);
        }

        public bool isKeyPressed(Keys key)
        {
            return newState.IsKeyDown(key) && !oldState.IsKeyDown(key);
        }
    }
}
