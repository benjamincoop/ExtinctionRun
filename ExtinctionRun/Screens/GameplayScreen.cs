using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ExtinctionRun.StateManagement;
using ExtinctionRun.Sprites;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ExtinctionRun.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;
        private readonly InputAction _jumpAction;
        private readonly InputAction _restartAction;

        public float Speed { get; set; } = Constants.RunSpeed;

        /// <summary>
        /// The player's current score
        /// </summary>
        private int _score = 0;

        /// <summary>
        /// Awards player points after a certain time interval has elapsed
        /// </summary>
        private double _scoreClock = 0;

        /// <summary>
        /// Spawns a new hazard after a certain time interval
        /// </summary>
        private double _spawnClock = 0;

        /// <summary>
        /// Keeps the player invulnerable for a bit
        /// </summary>
        private double _invulnerableClock = 0;

        /// <summary>
        /// Increase the speed after a certain time interval
        /// </summary>
        private double _speedClock = 0;

        /// <summary>
        /// the number of hazards to be spawned
        /// </summary>
        private int _spawnCount = 0;

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

        /// <summary>
        /// The collection of coin sprites
        /// </summary>
        private List<Coin> _coins;

        /// <summary>
        /// The collection of heart pickups
        /// </summary>
        private List<Heart> _heartPickups;

        /// <summary>
        /// Shows whether the player has an extra life or not
        /// </summary>
        private Heart _extraLife;

        /// <summary>
        /// Indicates that the player is temporarily invulnerable to hazards.
        /// </summary>
        private bool _invulnerable = false;

        private SoundEffect _SFXcoin;
        private SoundEffect _SFXheart;
        private SoundEffect _SFXhurt;
        private SoundEffect _SFXdead;
        private Song _music;

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
            );

            _restartAction = new InputAction(
                new[] {Buttons.Start},
                new[] {Keys.Enter},
                true
            );

            // Create the scrolling background
            _backgrounds = new Background[] {
                new Background(new Vector2(0,0)),
                new Background(new Vector2(Constants.GameWidth,0))
            };

            // Create the terrain tiles
            _terrainTiles = new Terrain[]
            {
                new Terrain(new Vector2(0,Constants.GameHeight-Constants.TerrainHeight), Speed),
                new Terrain(new Vector2(Constants.GameWidth, Constants.GameHeight-Constants.TerrainHeight), Speed)
            };

            _player = new Player(new Vector2(
                (Constants.GameWidth / 2) - (Constants.PlayerWidth * Constants.PlayerScale / 2),
                Constants.GameHeight - (Constants.TerrainHeight + Constants.PlayerHeight * Constants.PlayerScale))
            );

            _hazards = new List<Hazard>();
            _coins = new List<Coin>();
            _heartPickups = new List<Heart>();
            _extraLife = new Heart(new Vector2(Constants.GameWidth - (Constants.HeartSize + 5), 5), false, Speed);
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
            foreach (Terrain terrain in _terrainTiles) { terrain.LoadContent(_content); }
            _player.LoadContent(_content);
            //_hazards.Add(SpawnHazard());
            _coins.Add(SpawnCoin(new Vector2(Constants.GameWidth, Constants.GameHeight - (Constants.TerrainHeight + (Constants.CoinSize * Constants.CoinScale)))));
            //_heartPickups.Add(SpawnHeart(new Vector2(Constants.GameWidth, Constants.GameHeight - (Constants.TerrainHeight + Constants.HeartSize + 200))));
            _extraLife.LoadContent(_content);
            _SFXcoin = _content.Load<SoundEffect>("CoinPickup");
            _SFXhurt = _content.Load<SoundEffect>("Hurt");
            _SFXdead = _content.Load<SoundEffect>("Dead");
            _SFXheart = _content.Load<SoundEffect>("HeartPickup");
            _music = _content.Load<Song>("GameMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);

            // Pause for a bit to make the loading screen look cooler
            Thread.Sleep(500);
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
                if (_player.State != Player.PlayerState.DEAD)
                {
                    // update score
                    _scoreClock += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (_scoreClock >= 100)
                    {
                        _score += Constants.ScoreRate;
                        _scoreClock = 0;
                    }

                    // spawn hazards
                    if(_spawnCount > 0)
                    {
                        _spawnClock += gameTime.ElapsedGameTime.TotalMilliseconds;
                        if(_spawnClock >= 150)
                        {
                            _hazards.Add(SpawnHazard());
                            _spawnCount--;
                            _spawnClock = 0;
                        }
                    } else
                    {
                        if(_hazards.Count < 2)
                        {
                            _spawnCount = random.Next(1, 2);
                        }
                    }

                    //check for invulnerablity
                    if(_invulnerable)
                    {
                        _invulnerableClock += gameTime.ElapsedGameTime.TotalMilliseconds;
                        if(_invulnerableClock >= 500)
                        {
                            _invulnerable = false;
                            _invulnerableClock = 0;
                        }
                    }

                    //check for difficulty increase
                    _speedClock += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if(_speedClock >= 1000)
                    {
                        if(Speed > Constants.MaxSpeed)
                        {
                            Speed -= 1f;
                        }
                        _speedClock = 0;
                    }

                    // update sprites
                    foreach (Background bg in _backgrounds) {
                        bg.Update(gameTime);
                    }
                    foreach (Terrain terrain in _terrainTiles) {
                        terrain.Update(gameTime);
                        terrain.Velocity = new Vector2(Speed, 0f);
                    }
                    foreach (Hazard hazard in _hazards.ToArray())
                    {
                        hazard.Update(gameTime);
                        hazard.Velocity = new Vector2(Speed, 0f);
                        if (hazard.CollisionBox.CollidesWith(_player.CollisionBox))
                        {
                            _player.ShadingColor = Color.Red;
                            if (_invulnerable == false)
                            {
                                if (_extraLife.Active)
                                {
                                    _SFXhurt.Play();
                                    _extraLife.Active = false;
                                    _invulnerable = true;
                                }
                                else
                                {
                                    if (_player.State != Player.PlayerState.DYING)
                                    {
                                        _SFXdead.Play();
                                        GameOver();
                                    }
                                }
                            }
                        } else
                        {
                            _player.ShadingColor = Color.White;
                        }

                        if (hazard.Active == false) { _hazards.Remove(hazard); }
                    }
                    foreach (Coin coin in _coins.ToArray())
                    {
                        coin.Update(gameTime);
                        coin.Velocity = new Vector2(Speed, 0f);
                        if (coin.CollisionCircle.CollidesWith(_player.CollisionBox))
                        {
                            _score += Constants.CoinValue;
                            _SFXcoin.Play();
                            coin.Active = false;
                        }
                        if (coin.Active == false)
                        {
                            _coins.Remove(coin);
                            _coins.Add(SpawnCoin(new Vector2(Constants.GameWidth, Constants.GameHeight - (Constants.TerrainHeight + (Constants.CoinSize * Constants.CoinScale)))));
                        }
                    }
                    foreach (Heart heart in _heartPickups.ToArray())
                    {
                        heart.Update(gameTime);
                        heart.Velocity = new Vector2(Speed, 0f);
                        if (heart.CollisionCircle.CollidesWith(_player.CollisionBox))
                        {
                            heart.Active = false;
                            if (_extraLife.Active == false) {
                                _extraLife.Active = true;
                                _SFXheart.Play();
                            }
                        }
                        if (heart.Active == false)
                        {
                            _heartPickups.Remove(heart);
                        }
                    }
                    if (_extraLife.Active == false && _heartPickups.Count == 0)
                    {
                        int r = random.Next(1, 1000);
                        if(r == 1) { _heartPickups.Add(SpawnHeart(new Vector2(Constants.GameWidth, Constants.GameHeight - (Constants.TerrainHeight + Constants.HeartSize + 200)))); }
                        
                    }
                } else
                {
                    if (_player.State == Player.PlayerState.DEAD) { _player.ShadingColor = Color.White; } 
                }
                _player.Update(gameTime);
            }
        }

        // Controls input logic
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
                if(_restartAction.Occurred(input, ControllingPlayer, out player) && _player.State == Player.PlayerState.DEAD)
                {
                    LoadingScreen.Load(ScreenManager, true, ControllingPlayer.Value, new GameplayScreen());
                }
            }
        }

        // helper method for spawning a single random hazard
        private Hazard SpawnHazard()
        {
            Hazard hazard;
            int type = random.Next(0, 5);
            hazard = new Hazard(Vector2.Zero, (Hazard.HazardType)type, Speed);
            hazard.LoadContent(_content);
            return hazard;
        }

        // helper method for spawning a coin
        private Coin SpawnCoin(Vector2 position)
        {
            Coin coin = new Coin(position, Speed);
            coin.LoadContent(_content);
            return coin;
        }

        // helper method for spawning a heart
        private Heart SpawnHeart(Vector2 position)
        {
            Heart heart = new Heart(position, true, Speed);
            heart.LoadContent(_content);
            return heart;
        }

        /// <summary>
        /// Triggers a extinction event *ba-dum tiss*
        /// </summary>
        private void GameOver()
        {
            _player.State = Player.PlayerState.DYING;
        }

        public override void Draw(GameTime gameTime)
        {
            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.FromNonPremultiplied(140,91,0,255), 0, 0);
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // do drawing stuff
            foreach (Background bg in _backgrounds) { bg.Draw(spriteBatch); }
            foreach (Terrain terrain in _terrainTiles) { terrain.Draw(spriteBatch); }
            if (IsActive) { _player.Draw(spriteBatch); }
            foreach (Hazard hazard in _hazards) { hazard.Draw(spriteBatch); }
            foreach (Coin coin in _coins) { coin.Draw(spriteBatch); }
            foreach (Heart heart in _heartPickups) { heart.Draw(spriteBatch); }
            if(_extraLife.Active) { _extraLife.Draw(spriteBatch); }
            spriteBatch.DrawString(_gameFont, "Score:   " + _score.ToString(), Vector2.Zero, Color.Black);
            if(_player.State == Player.PlayerState.DEAD)
            {
                string str = "Game Over\nPress ENTER to restart.";
                Vector2 strSize = _gameFont.MeasureString(str);
                spriteBatch.DrawString(_gameFont, str, new Vector2(Constants.GameWidth / 2 - strSize.X / 2, Constants.GameHeight / 2 - strSize.Y), Color.Red);
            }
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
