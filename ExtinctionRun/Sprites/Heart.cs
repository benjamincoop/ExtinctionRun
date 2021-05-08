using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun.Sprites
{
    class Heart : Sprite
    {

        /// <summary>
        /// The rate at which the heart scrolls leftward
        /// </summary>
        public Vector2 Velocity { get; set; }

        public bool Active { get; set; } = true;

        /// <summary>
        /// Indicates if the heart is a pickup or just for the HUD
        /// </summary>
        public bool IsPickup { get; set; }

        /// <summary>
        /// Creates the heart sprite
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        public Heart(Vector2 position, bool isPickup, float speed)
        {
            Position = position;
            IsPickup = isPickup;
            CollisionCircle = new CollisionHelper.BoundingCircle(Vector2.Zero, Constants.HeartSize / 2);
            Velocity = new Vector2(speed, 0f);
        }

        /// <summary>
        /// Loads the heart texture
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            BaseTexture = content.Load<Texture2D>("Heart");
        }

        /// <summary>
        /// Draws the heart
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (BaseTexture is null)
            {
                throw new InvalidOperationException("Heart texture unloaded.");
            }
            else
            {
                spriteBatch.Draw(BaseTexture, Position, ShadingColor);
            }

        }

        /// <summary>
        /// If heart is pickup, move it left, otherwise draw it at top of screen
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if(IsPickup)
            {
                if (Position.X <= -1 * BaseTexture.Width) { Active = false; }

                Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                CollisionCircle.Center = new Vector2(Position.X + (Constants.HeartSize / 2), Position.Y + (Constants.HeartSize / 2));
            } else
            {
                Position = new Vector2(Constants.GameWidth - Constants.HeartSize - 5, 5);
                CollisionCircle.Center = Position;
            }
        }
    }
}
