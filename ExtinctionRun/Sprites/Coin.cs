using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun.Sprites
{
    class Coin : Sprite
    {

        /// <summary>
        /// The rate at which the coin scrolls leftward
        /// </summary>
        public Vector2 Velocity { get; set; }

        public bool Active { get; set; } = true;

        private Animation _animation;

        /// <summary>
        /// Creates a new coin sprite
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        public Coin(Vector2 position, float speed)
        {
            Position = position;
            ScaleFactor = Constants.CoinScale;
            CollisionCircle = new CollisionHelper.BoundingCircle(Vector2.Zero, (Constants.CoinSize * Constants.CoinScale) / 2);
            Velocity = new Vector2(speed, 0f);
        }

        /// <summary>
        /// Loads the coin texture
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            _animation = new Animation(content, new string[] { "Coin (1)", "Coin (2)", "Coin (3)", "Coin (4)", "Coin (5)", "Coin (6)", "Coin (7)", "Coin (8)" }, 5);
        }

        /// <summary>
        /// Draws the coin at its current position
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            BaseTexture = _animation.Animate();
            if (BaseTexture is null)
            {
                throw new InvalidOperationException("Hazard texture unloaded.");
            }
            else
            {
                spriteBatch.Draw(BaseTexture, Position, null, ShadingColor, Rotation, Vector2.Zero, ScaleFactor, SpriteEffects.None, 0);
            }

        }

        /// <summary>
        /// Moves the sprite leftwards at a fixed speed and handles screen wraparound
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (Position.X <= -1 * BaseTexture.Width) { Active = false; }

            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            CollisionCircle.Center = new Vector2(Position.X + ((Constants.CoinSize * Constants.CoinScale) / 2), Position.Y + ((Constants.CoinSize * Constants.CoinScale) / 2));
        }
    }
}
