using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PacmanGame.Entities
{
    interface Entity
    {
        void update(double dt);

        void draw(SpriteBatch sb);

        Vector2 getPosition();
    }
}
