using System;
using Microsoft.Xna.Framework;

namespace MonoGameSandbox
{
    public struct SpriteBounds
    {
        public Vector2 UpperLeft;
        public int Width, Height;

        /// <summary>
        /// Update the sprite bounds so that location is the centre of the bottom of the bounds.
        /// </summary>
        /// <param name="location">Location.</param>
        public void CentreAtLocation(Vector2 location)
        {
            float x = location.X + Constants.TileSize / 2;
            float y = location.Y + Constants.TileSize;
            UpperLeft = new Vector2(x - Width / 2f, y - Height);
        }
    }
}

