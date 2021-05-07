using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ExtinctionRun.StateManagement;
using ExtinctionRun.Sprites;
using System.Collections.Generic;

namespace ExtinctionRun.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;
        private readonly InputAction _jumpAction;

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

        /// <summary>
        /// The collection of hazard sprites
        /// </summary>
        private List<Hazard> _hazards;

        private Random random = new Random();

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape },
                true
            );

            _jumpAction = new InputAction(
                new[] { Buttons.A, Buttons.DPadUp },
                new[] { Keys.Space},
                true
            ) ;

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

            _player = new Player(new Vector2(
                (Constants.GameWidth / 2) - (Constants.PlayerWidth * Constants.PlayerScale / 2),
                Constants.GameHeight - (Constants.TerrainHeight + Constants.PlayerHeight * Constants.PlayerScale))
            );

            _hazards = new List<Hazard>();
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
            _hazards.Add(SpawnHazard());

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
                foreach(Hazard h in _hazards.ToArray()) {
                    h.Update(gameTime);
                    if(h.Active == false)
                    {
                        _hazards.Remove(h);
                        _hazards.Add(SpawnHazard());
                    }
                }
                _player.Update(gameTime);
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
                if(_jumpAction.Occurred(input, ControllingPlayer, out player) && _player.State == Player.PlayerState.RUNNING)
                {
                    _player.Jump();
                }
            }
        }

        private Hazard SpawnHazard()
        {
            Hazard hazard;
            int type = random.Next(0, 5);
            hazard = new Hazard(Vector2.Zero, (Hazard.HazardType)type);
            hazard.LoadContent(_content);
            return hazard;
        }

        public override void Draw(GameTime gameTime)
        {
            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // do drawing stuff
            foreach (Background bg in _backgrounds) { bg.Draw(spriteBatch); }
            foreach (Terrain t in _terrainTiles) { t.Draw(spriteBatch); }
            if (IsActive) { _player.Draw(spriteBatch); }
            foreach (Hazard h in _hazards) { h.Draw(spriteBatch); }

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
