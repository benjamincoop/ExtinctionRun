using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun.Sprites
{
    public class Terrain : Sprite
    {
        #region Local vars
        /// <summary>
        /// The amount of overlap between tiles, to hide the seam between them
        /// </summary>
        readonly int xOffset = 10;
        #endregion

        /// <summary>
        /// The rate at which the tile scrolls leftward
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Creates a new terrain tile
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        public Terrain(Vector2 position, float speed)
        {
            Position = position;
            Velocity = new Vector2(speed, 0f);
        }

        /// <summary>
        /// Loads the terrain texture
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            BaseTexture = content.Load<Texture2D>("terrain_platform");
        }

        /// <summary>
        /// Draws the tile at its current position
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (BaseTexture is null)
            {
                throw new InvalidOperationException("Terrain texture unloaded.");
            }
            else
            {
                spriteBatch.Draw(BaseTexture, Position, ShadingColor);
            }

        }

        /// <summary>
        /// Moves the sprite leftwards at a fixed speed and handles screen wraparound
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (Position.X - xOffset <= -1 * BaseTexture.Width) { Position = new Vector2(Constants.GameWidth, Constants.GameHeight - Constants.TerrainHeight); }

            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
