using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox
{
    public class RPGBaseSprite
    {
        private int currentRow;
        private string name;
        protected Texture2D texture;
        protected int columns, rows, currentFrame;
        protected bool animated;
        protected int animationSpeed, animationCount;
        protected SpriteBounds bounds;
        protected Vector2 location;

        public bool Passible
        {
            get;
            set;
        }

        public Vector2 Location
        {
            get{ return location; }
            set
            {
                location = value;
            }
        }

        public enum Speed
        {
            VeryFast = 1,
            Fast,
            Normal,
            Slow,
            VerySlow
        }

        public RPGBaseSprite(string name, Texture2D texture, int columns, int rows)
        {
            this.name = name;
            this.texture = texture;
            this.columns = columns;
            this.rows = rows;
            
            currentRow = 0;
            currentFrame = 0;
            
            animated = false;
            animationSpeed = (int)Speed.Normal;
            animationCount = 0;

            location = Vector2.Zero;
            bounds.Width = texture.Width / columns;
            bounds.Height = texture.Height / rows;
            bounds.CentreAtLocation(Location);
        }

        public void SetAnimationProperties(bool animated, int startRow, int startColumn, Speed newAnimationSpeed = Speed.Normal)
        {
            this.animated = animated;
            this.currentRow = startRow;
            currentFrame = startColumn;
            animationSpeed = (int)newAnimationSpeed;
        }

        public void SetLocationByTile(int x, int y)
        {
            location = new Vector2(x * Constants.TileSize, y * Constants.TileSize);
        }

        public virtual void Update()
        {
            if (animated)
            {
                if (animationCount % (animationSpeed * 4) == 0)
                {
                    currentFrame = (currentFrame + 1) % columns;
                    animationCount = 0;
                }
                animationCount++;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            bounds.CentreAtLocation(location);
            if (!GameOfRPG.Camera.SpriteIsVisible(bounds))
                return;
            int x = currentFrame * bounds.Width;
            int y = currentRow * bounds.Height;
            Rectangle sourceRectangle = new Rectangle(x, y, bounds.Width, bounds.Height);
            Vector2 drawLocation = GameOfRPG.Camera.GetScreenLocation(bounds.UpperLeft);
            //spriteBatch.Begin(SpriteSortMode.Deferred,null,null,null,null,null,GameOfRPG.Variables.Scale);
            spriteBatch.Draw(texture, drawLocation, sourceRectangle, Color.White);
            //spriteBatch.End();
        }
    }
}

