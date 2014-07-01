using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox
{
    public class FieldOfView
    {
        private static Vector2 screenCentre = new Vector2(Constants.PlayAreaWidth / 2 - Constants.TileSize,
                                                  Constants.ScreenHeight / 2 - Constants.TileSize);
        private Vector2 upperLeft, bottomRight, currentPlayerLocation;
        private GameMap map;
        private MapEffects mapEffects;

        public Vector2 PlayerLocation{ get { return currentPlayerLocation; } }

        public FieldOfView()
        {
            mapEffects = new MapEffects();
            upperLeft = Vector2.Zero;
            bottomRight = new Vector2(Constants.PlayAreaWidth, Constants.ScreenHeight);
        }

        public void InitializeToMap(GameMap newMap)
        {
            map = newMap;
        }

        public void Update(Vector2 playerLocation)
        {
            currentPlayerLocation = playerLocation;
            upperLeft = Vector2.Subtract(playerLocation, screenCentre);
            if (upperLeft.X < 0)
                upperLeft.X = 0f;
            else if (upperLeft.X > map.RealWidth - Constants.PlayAreaWidth)
                upperLeft.X = map.RealWidth - Constants.PlayAreaWidth;
            if (upperLeft.Y < 0)
                upperLeft.Y = 0f;
            else if (upperLeft.Y > map.RealHeight - Constants.ScreenHeight)
                upperLeft.Y = map.RealHeight - Constants.ScreenHeight;
            bottomRight = Vector2.Add(upperLeft, new Vector2(Constants.PlayAreaWidth, Constants.ScreenHeight));
            map.Update();
        }

        public void Draw(SpriteBatch sb, RPGPlayerSprite player){
            map.Draw(sb, player);
            //    mapEffects.Draw(sb);
        }


        public bool LocationIsValid(Vector2 location)
        {
            if (location.X < 0 || location.X > map.RealWidth - Constants.TileSize)
                return false;
            if (location.Y < 0 || location.Y > map.RealHeight - Constants.TileSize)
                return false;
            return true;
        }

        public bool LocationIsVisible(Vector2 location)
        {
            if (!LocationIsValid(location))
                return false;
            if (location.X < upperLeft.X - Constants.TileSize || location.X > bottomRight.X + Constants.TileSize)
                return false;
            if (location.Y < upperLeft.Y - Constants.TileSize || location.Y > bottomRight.Y + Constants.TileSize)
                return false;
            return true;
        }

        public bool SpriteIsVisible(SpriteBounds spriteBounds)
        {
            float x_low = spriteBounds.UpperLeft.X;
            float y_low = spriteBounds.UpperLeft.Y;
            float x_high = x_low + spriteBounds.Width;
            float y_high = y_low + spriteBounds.Height;
            if (x_high <= upperLeft.X)
                return false;
            if (x_low >= bottomRight.X)
                return false;
            if (y_high <= upperLeft.Y)
                return false;
            if (y_low >= bottomRight.Y)
                return false;
            return true;
        }

        public void CheckTriggers(Vector2 location, params TriggerType [] triggers){
            if (LocationIsValid(location)){
                map.CheckTriggers(location, triggers);
            }
        }

        public bool LocationIsPassible(Vector2 previousLocation, Vector2 newLocation)
        {
            return (map.CheckPassability(previousLocation, newLocation));
        }

        public Vector2 GetScreenLocation(Vector2 location)
        {
            return Vector2.Subtract(location, upperLeft);
        }

        public Point GetFirstVisibleTileLocation(){
            int x = (int)upperLeft.X - Constants.TileSize;
            if (x < 0)
                x = 0;
            int y = (int)upperLeft.Y - Constants.TileSize;
            if (y < 0)
                y = 0;
            return new Point(x / Constants.TileSize, y / Constants.TileSize);
        }
        // TODO: remove Debug stuff in final iteration
        [Debug]
        public string DebugSnapShot(){
            string time = System.DateTime.Now.ToLongTimeString();
            string playerPosition = String.Format("\n   @({0},{1}) ", (int)currentPlayerLocation.X / Constants.TileSize, (int)currentPlayerLocation.Y / Constants.TileSize);
            string mapname = map.Name;
            return playerPosition + "in " + mapname + "    " + time;
        }
    }
}

