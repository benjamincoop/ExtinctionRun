using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExtinctionRun.Sprites
{
    public class Hazard : Sprite
    {
        public enum HazardType
        {
            SHORT_CACTUS,
            MEDIUM_CACTUS,
            TALL_CACTUS,
            BUSH,
            STONE
        }

        /// <summary>
        /// Gives the type of the hazard
        /// </summary>
        public HazardType Type { get; set; }

        /// <summary>
        /// The rate at which the sprite scrolls leftward
        /// </summary>
        public Vector2 Velocity { get; set; } = new Vector2(Constants.RunSpeed, 0f);

        public bool Active { get; set; } = true;

        /// <summary>
        /// Creates a new hazard sprite
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        public Hazard(Vector2 position, HazardType type)
        {
            Position = position;
            Type = type;
        }

        /// <summary>
        /// Loads the hazard texture
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            switch(Type)
            {
                case HazardType.SHORT_CACTUS:
                    BaseTexture = content.Load<Texture2D>("Cactus (2)");
                    break;
                case HazardType.MEDIUM_CACTUS:
                    BaseTexture = content.Load<Texture2D>("Cactus (3)");
                    break;
                case HazardType.TALL_CACTUS:
                    BaseTexture = content.Load<Texture2D>("Cactus (1)");
                    break;
                case HazardType.BUSH:
                    BaseTexture = content.Load<Texture2D>("Bush");
                    break;
                case HazardType.STONE:
                    BaseTexture = content.Load<Texture2D>("Stone");
                    break;
            }
            Position = new Vector2(Constants.GameWidth, Constants.GameHeight - (Constants.TerrainHeight + BaseTexture.Height));
            CollisionBox = new CollisionHelper.BoundingRectangle(Position, new Vector2(BaseTexture.Width, BaseTexture.Height));
        }

        /// <summary>
        /// Draws the sprite at its current position
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (BaseTexture is null)
            {
                throw new InvalidOperationException("Hazard texture unloaded.");
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
            if (Position.X <= -1 * BaseTexture.Width) { Active = false; }

            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            CollisionBox.Position = Position;
        }
    }
}
