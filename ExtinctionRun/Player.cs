using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun
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

        private Animation[] _animations;
        private int _animIndex = 0;

        /// <summary>
        /// Create a new player sprite
        /// </summary>
        /// <param name="position"></param>
        public Player(Vector2 position)
        {
            Position = position;
            ScaleFactor = 0.25f;
        }

        /// <summary>
        /// Loads player textures/animations
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            _animations = new Animation[1];
            _animations[0] = new Animation(content, new string[]{ "Run (1)", "Run (2)", "Run (3)", "Run (4)", "Run (5)", "Run (6)", "Run (7)", "Run (8)" }, 5);
        }

        /// <summary>
        /// Draws the tile at its current position
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            BaseTexture = _animations[_animIndex].Animate();
            if (BaseTexture is null)
            {
                throw new InvalidOperationException("Player texture unloaded.");
            }
            else
            {
                spriteBatch.Draw(BaseTexture, Position, null, ShadingColor, Rotation, Vector2.Zero, ScaleFactor, SpriteEffects.None, 1);
            }

        }

        /// <summary>
        /// Moves the sprite leftwards at a fixed speed and handles screen wraparound
        /// </summary>
        public void Update(GameTime gameTime)
        {

        }
    }
}
