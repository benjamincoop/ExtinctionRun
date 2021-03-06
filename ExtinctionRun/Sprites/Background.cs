using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun.Sprites
{
    public class Background : Sprite
    {
        #region Local vars
        /// <summary>
        /// The amount of overlap between backgrounds, to hide the seam between them
        /// </summary>
        readonly int xOffset = 10;
        #endregion

        /// <summary>
        /// The rate at which the background scrolls leftward
        /// </summary>
        public Vector2 Velocity { get; set; } = new Vector2(-200f, 0f);

        /// <summary>
        /// Constructs a new class instance
        /// </summary>
        /// <param name="game">The game this ball belongs in</param>
        /// <param name="color">A color to distinguish this ball</param>
        public Background(Vector2 position)
        {
            Position = position;
        }

        /// <summary>
        /// Loads the background texture
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            BaseTexture = content.Load<Texture2D>("desert_bg");
        }

        /// <summary>
        /// Draws the background sprite at its current position
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (BaseTexture is null)
            {
                throw new InvalidOperationException("Background texture unloaded.");
            }
            else
            {
                spriteBatch.Draw(BaseTexture, Position, ShadingColor);
            }

        }

        /// <summary>
        /// Moves the background sprite leftwards at a fixed speed and handles screen wraparound
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (Position.X - xOffset <= -1 * BaseTexture.Width) { Position = new Vector2(Constants.GameWidth, 0); }

            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
