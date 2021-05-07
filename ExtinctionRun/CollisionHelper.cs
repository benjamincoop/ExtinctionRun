using System;
using Microsoft.Xna.Framework;

namespace ExtinctionRun
{
    /// <summary>
    /// Provides a framework for detecting collisions between sprites
    /// </summary>
    public class CollisionHelper
    {
        /// <summary>
        /// Defines a rectangular-type collision box
        /// </summary>
        public class BoundingRectangle
        {
            /// <summary>
            /// The width and height of the bounding box, after scaling
            /// </summary>
            public Vector2 Size { get; set; }

            /// <summary>
            /// The position of the bounding box
            /// </summary>
            public Vector2 Position { get; set; }

            /// <summary>
            /// Creates a BoundingRectangle around the given sprite
            /// </summary>
            /// <param name="sprite"></param>
            public BoundingRectangle(Vector2 position, Vector2 size)
            {
                Position = position;
                Size = size;
            }

            /// <summary>
            /// Detects if this BoundingRectangle is colliding with another
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public bool CollidesWith(BoundingRectangle other)
            {
                return !(Position.X + Size.X < other.Position.X
                    || Position.X > other.Position.X + other.Size.X
                    || Position.Y + Size.Y < other.Position.Y
                    || Position.Y > other.Position.Y + other.Size.Y);
            }

            /// <summary>
            /// Detects if this BoundingRectangle is colliding with a BoundingCircle
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool CollidesWith(BoundingCircle other)
            {
                float nearestX = MathHelper.Clamp(other.Center.X, Position.X, Position.X + Size.X);
                float nearestY = MathHelper.Clamp(other.Center.Y, Position.Y, Position.Y + Size.Y);
                return Math.Pow(other.Radius, 2) >=
                    Math.Pow(other.Center.X - nearestX, 2) +
                    Math.Pow(other.Center.Y - nearestY, 2);
            }
        }

        public class BoundingCircle
        {
            /// <summary>
            /// The center point of the bounding circle
            /// </summary>
            public Vector2 Center { get; set; }

            /// <summary>
            /// The radius of the bounding circle
            /// </summary>
            public float Radius { get; set; }

            /// <summary>
            /// Constructs a new bounding circle
            /// </summary>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            public BoundingCircle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            /// <summary>
            /// Detects if this BoundingCircle is colliding with another
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool CollidesWith(BoundingCircle other)
            {
                return Math.Pow(Radius + other.Radius, 2) >=
                Math.Pow(Center.X - other.Center.X, 2) +
                Math.Pow(Center.Y - other.Center.Y, 2);
            }

            /// <summary>
            /// Detects if BoundingCircle is colliding with BoundingRectangle
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool CollidesWith(BoundingRectangle other)
            {
                return other.CollidesWith(this);
            }
        }
    }
}
