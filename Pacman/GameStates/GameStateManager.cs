using Microsoft.Xna.Framework.Graphics;
using PacmanGame.Maps;

namespace PacmanGame.GameStates
{
    class GameStateManager
    {
        Game game;
        InputProcessor input;
        Texture2D texAtlas;

        SoundBank soundBank;
        MapManager mapManager;

        public enum States { INTRO, SELECT_MAP, PLAY, CREDITS};
        GameState currState;

        public GameStateManager(Game game, InputProcessor input, SoundBank soundBank, Texture2D texAtlas)
        {
            this.game = game;
            this.input = input;
            this.soundBank = soundBank;
            this.texAtlas = texAtlas;

            mapManager = new MapManager(texAtlas);

            setState(States.INTRO, "");
        }

        public void update(double dt)
        {
            currState.handleInput(input);
            currState.update(dt);
        }

        public void render(SpriteBatch sb)
        {
            currState.render(sb);
        }

        public void setState(States state, string arg)
        {
            soundBank.stopAll();
            switch (state)
            {
                case States.INTRO: currState = new IntroState(this, soundBank, texAtlas);
                    break;
                case States.CREDITS:
                    currState = new CreditsState(this, soundBank, texAtlas);
                    break;
                case States.SELECT_MAP: currState = new SelectMapState(this, soundBank, texAtlas, mapManager);
                    break;
                case States.PLAY: currState = new PlayState(this, soundBank, mapManager, arg, texAtlas);
                    break;
                default:
                    break;
            }

        }

        public void exit()
        {
            game.Exit();
        }

        public Game GetGame
        {
            get { return game; }
        }
    }
}
