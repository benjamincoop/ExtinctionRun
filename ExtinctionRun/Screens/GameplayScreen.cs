using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ExtinctionRun.StateManagement;
using ExtinctionRun.Sprites;

namespace ExtinctionRun.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        /// <summary>
        /// The two sprites that make up the infinite scrolling background
        /// </summary>
        private Background[] _backgrounds;

        /// <summary>
        /// The tiles that make up the scrolling terrain platform
        /// </summary>
        private Terrain[] _terrainTiles;

        /// <summary>
        /// The player sprite
        /// </summary>
        private Player _player;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);

            // Create the scrolling background
            _backgrounds = new Background[] {
                new Background(new Vector2(0,0)),
                new Background(new Vector2(Constants.GameWidth,0))
            };

            // Create the terrain tiles
            _terrainTiles = new Terrain[]
            {
                new Terrain(new Vector2(0,Constants.GameHeight-Constants.TerrainHeight)),
                new Terrain(new Vector2(Constants.GameWidth, Constants.GameHeight-Constants.TerrainHeight))
            };

            _player = new Player(new Vector2(Constants.GameWidth / 2, Constants.GameHeight - (Constants.TerrainHeight + Constants.PlayerHeight)));
        }

        // Load assets
        public override void Activate()
        {
            if (_content == null)
            {
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            // Load assets
            _gameFont = _content.Load<SpriteFont>("gamefont");
            foreach (Background bg in _backgrounds) { bg.LoadContent(_content); }
            foreach(Terrain t in _terrainTiles) { t.LoadContent(_content); }
            _player.LoadContent(_content);

            // Pause for a second to make the loading screen look cooler
            Thread.Sleep(1000);
            ScreenManager.Game.ResetElapsedTime();
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // do game stuff
                foreach(Background bg in _backgrounds) { bg.Update(gameTime); }
                foreach(Terrain t in _terrainTiles) { t.Update(gameTime); }
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // do control stuff
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // do drawing stuff
            foreach (Background bg in _backgrounds) { bg.Draw(spriteBatch); }
            foreach(Terrain t in _terrainTiles) { t.Draw(spriteBatch); }
            _player.Draw(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
