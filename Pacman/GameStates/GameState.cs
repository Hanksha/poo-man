using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.GameStates
{
    interface GameState
    {
        void handleInput(InputProcessor input);

        void update(double dt);

        void render(SpriteBatch sb);
    }
}
