using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun.Sprites
{
    class Player : Sprite
    {
        public enum PlayerState
        {
            RUNNING,
            JUMPING,
            FALLING,
            DYING,
            DEAD
        }

        private Animation[] _animations; // the collection of player animations, can be indexed with the PlayerState

        /// <summary>
        /// Gives the current gameplay/animation state of the player
        /// </summary>
        public PlayerState State { get; set; } = PlayerState.RUNNING;

        /// <summary>
        /// Represents a force applied to the player
        /// </summary>
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        /// <summary>
        /// Create a new player sprite
        /// </summary>
        /// <param name="position"></param>
        public Player(Vector2 position)
        {
            Position = position;
            ScaleFactor = Constants.PlayerScale;
            CollisionBox = new CollisionHelper.BoundingRectangle(Position, Vector2.Zero);
        }

        /// <summary>
        /// Loads player textures/animations
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            _animations = new Animation[5];
            _animations[(int)PlayerState.RUNNING] = new Animation(content, new string[] { "Run (1)", "Run (2)", "Run (3)", "Run (4)", "Run (5)", "Run (6)", "Run (7)", "Run (8)" }, 5);
            _animations[(int)PlayerState.JUMPING] = new Animation(content, new string[] { "Jump (1)", "Jump (2)", "Jump (3)", "Jump (4)", "Jump (5)", "Jump (6)", "Jump (7)", "Jump (8)", "Jump (9)", "Jump (10)", "Jump (11)", "Jump (12)" }, 3);
            _animations[(int)PlayerState.FALLING] = new Animation(content, new string[] { "Jump (12)" }, 0);
            _animations[(int)PlayerState.DYING] = new Animation(content, new string[] { "Dead (1)", "Dead (2)", "Dead (3)", "Dead (4)", "Dead (5)", "Dead (6)", "Dead (7)", "Dead (8)" }, 5);
            _animations[(int)PlayerState.DEAD] = new Animation(content, new string[] { "Dead (8)" }, 0);
        }

        /// <summary>
        /// Draws the sprite at its current position
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            
            // Check if death animation is finished
            if(State == PlayerState.DYING && _animations[(int)State].IsFinished)
            {
                State = PlayerState.DEAD;
            }

            // Get current texture
            BaseTexture = _animations[(int)State].Animate();

            // set collision bounds to size of texture
            CollisionBox.Size = new Vector2(BaseTexture.Width, BaseTexture.Height) * (ScaleFactor * 0.75f);

            if (BaseTexture is null)
            {
                throw new InvalidOperationException("Player texture unloaded.");
            }
            else
            {
                spriteBatch.Draw(BaseTexture, Position, null, ShadingColor, Rotation, Vector2.Zero, ScaleFactor, SpriteEffects.None, 0);
            }

        }

        /// <summary>
        /// Starts the player jump sequence
        /// </summary>
        public void Jump()
        {
            State = PlayerState.JUMPING;
            _animations[(int)State].Reset();
            Velocity = new Vector2(0, Constants.ForceJump);
        }

        /// <summary>
        /// Player update logic
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if(State == PlayerState.JUMPING || State == PlayerState.FALLING)
            {
                // Apply gravity and calculate new player position
                Velocity += new Vector2(0, Constants.ForceGravity);
                Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Check if player is falling
                if(Velocity.Y >= 0) { State = PlayerState.FALLING; }

                // Reset player state and position upon landing
                if(State == PlayerState.FALLING && Position.Y >= Constants.GameHeight - (Constants.TerrainHeight + Constants.PlayerHeight * Constants.PlayerScale))
                {
                    State = PlayerState.RUNNING;
                    Velocity = Vector2.Zero;
                    Position = new Vector2(
                        (Constants.GameWidth / 2) - (Constants.PlayerWidth * Constants.PlayerScale / 2),
                        Constants.GameHeight - (Constants.TerrainHeight + Constants.PlayerHeight * Constants.PlayerScale)
                    );
                }
            } 
            // update collisionbox position to match player's
            CollisionBox.Position = Position;
        }
    }
}
