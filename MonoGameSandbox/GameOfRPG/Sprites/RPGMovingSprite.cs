using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameSandbox
{
    public class RPGMovingSprite : RPGBaseSprite
    {
        #region Fields

        protected RPGDirection direction;
        protected Vector2 movingLocation, previousLocation;
        protected bool moving, waiting;

        private bool repeatMovement;
        private int moveDistance, currentMove;
        private List<int> movements;

        #endregion

        #region Enums

        protected enum RPGDirection
        {
            Down,
            Right,
            Up,
            Left
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameSandbox.RPGMovingSprite"/> class.
        /// A moving sprite has 4 rows in the texture, directions facing down, right, up, and left, in that order.
        /// </summary>
        /// <param name="name">Sprite Name.</param>
        /// <param name="texture">Sprite Texture.</param>
        /// <param name="columns">Columns in the texture.</param>
        public RPGMovingSprite(string name, Texture2D texture, int columns) : base(name, texture, columns, 4)
        {
            direction = RPGDirection.Down;
            currentFrame = 0;
            moving = false;
            waiting = false;

            moveDistance = Constants.TileSize / 4;
            movements = new List<int>();
            repeatMovement = false;
            currentMove = -1;
        }

        protected Vector2 GetNextLocation(RPGDirection newDirection)
        {
            Vector2 newLocation = location;
            switch (newDirection)
            {
                case(RPGDirection.Down):
                    newLocation.Y += Constants.TileSize;
                    break;
                case(RPGDirection.Left):
                    newLocation.X -= Constants.TileSize;
                    break;
                case(RPGDirection.Up):
                    newLocation.Y -= Constants.TileSize;
                    break;
                case(RPGDirection.Right):
                    newLocation.X += Constants.TileSize;
                    break;
            }
            return newLocation;
        }

        protected void MoveDirection(RPGDirection newDirection)
        {
            if (moving)
                return;
            FaceDirection(newDirection);
            Vector2 newLocation = GetNextLocation(newDirection);
            if (GameOfRPG.Camera.LocationIsValid(newLocation))
            {
                if (Passible || GameOfRPG.Camera.LocationIsPassible(location, newLocation))
                {
                    moving = true;
                    waiting = false;
                    previousLocation = location;
                    movingLocation = newLocation;
                }
               
            }
        }

        protected void FaceDirection(RPGDirection newDirection)
        {
            direction = newDirection;
            waiting = true;
        }

        public void FacePlayerForInterpreter(){
            FaceDirection(GetPlayerDirection());
        }


        public void SetMovementPattern(bool repeatable, params int[] newMovements)
        {
            repeatMovement = repeatable;
            movements.Clear();
            currentMove = 0;
            movements.AddRange(newMovements);
        }
        //TODO: Do not know if this is neccessary.
        public void AddToMovementPattern(params int[] newMovements)
        {
            movements.AddRange(newMovements);
        }

        private void DoNextMovement()
        {
            int moveCommand = movements[currentMove];
            switch (moveCommand)
            {
                case(0):    // Move Down
                case(1):    // Move Right
                case(2):    // Move Up
                case(3):    // Move Left
                    MoveDirection((RPGDirection)moveCommand);
                    break;
                case(4):    // Move Random
                    MoveDirection((RPGDirection)GameOfRPG.randomGenerator.Next(4));
                    break;
                case(5):   // Move Towards
                    MoveDirection(GetPlayerDirection());
                    break;
                case(6):   // Move Away
                    MoveDirection(GetOppositePlayerDirection());
                    break;
                case(7):    // Face Down
                case(8):    // Face Right
                case(9):    // Face Up
                case(10):    // Face Left
                    FaceDirection((RPGDirection)(moveCommand - 7));
                    break;
                case(11):    // Face Random
                    FaceDirection((RPGDirection)GameOfRPG.randomGenerator.Next(4));
                    break;
                case(12):   // Face Towards
                    FaceDirection(GetPlayerDirection());
                    break;
                case(13):   // Face Away
                    FaceDirection(GetOppositePlayerDirection());
                    break;
                case(14):   // Waiting
                    waiting = true;
                    break;
            }
            currentMove++;
            if (currentMove == movements.Count)
            {
                if (repeatMovement)
                    currentMove = 0;
                else
                    currentMove = -1;
            }
        }

        private RPGDirection GetOppositePlayerDirection()
        {
            //This looks more confusing than it is.
            Vector2 locationDifference = Vector2.Subtract(GameOfRPG.Camera.PlayerLocation, location);
            RPGDirection newDirection;
            if (Math.Abs(locationDifference.Y) > Math.Abs(locationDifference.X))
            {
                if (locationDifference.X > 0)
                    newDirection = RPGDirection.Left;
                else if (locationDifference.X < 0)
                    newDirection = RPGDirection.Right;
                else
                    newDirection = (GameOfRPG.randomGenerator.Next(1) == 0) ? RPGDirection.Left : RPGDirection.Right;
                if (GameOfRPG.Camera.LocationIsValid(GetNextLocation(newDirection)))
                    return newDirection;
                if (locationDifference.Y > 0)
                    newDirection = RPGDirection.Up;
                else
                    newDirection = RPGDirection.Down;
                if (GameOfRPG.Camera.LocationIsValid(GetNextLocation(newDirection)))
                    return newDirection;
                
            }
            else if (Math.Abs(locationDifference.X) > Math.Abs(locationDifference.Y))
            {
                if (locationDifference.Y > 0)
                    newDirection = RPGDirection.Up;
                else if (locationDifference.Y < 0)
                    newDirection = RPGDirection.Down;
                else
                    newDirection = (GameOfRPG.randomGenerator.Next(1) == 0) ? RPGDirection.Up : RPGDirection.Down;
                if (GameOfRPG.Camera.LocationIsValid(GetNextLocation(newDirection)))
                    return newDirection;
                if (locationDifference.X > 0)
                    newDirection = RPGDirection.Left;
                else
                    newDirection = RPGDirection.Right;
                if (GameOfRPG.Camera.LocationIsValid(GetNextLocation(newDirection)))
                    return newDirection;
            }
            return (RPGDirection)GameOfRPG.randomGenerator.Next(4);
        }

        private RPGDirection GetPlayerDirection()
        {
            Vector2 locationDifference = Vector2.Subtract(GameOfRPG.Camera.PlayerLocation, location);
            if (Math.Abs(locationDifference.X) > Math.Abs(locationDifference.Y))
            {
                if (locationDifference.X > 0)
                    return RPGDirection.Right;
                return RPGDirection.Left;
                
            }
            if (locationDifference.Y > 0)
                return RPGDirection.Down;
            else if (locationDifference.Y < 0)
                return RPGDirection.Up;
            // player and the sprite share the same location
            return (RPGDirection)GameOfRPG.randomGenerator.Next(4);        
        }

        public override void Update()
        {
            if (waiting)
            {
                if (animationCount % (animationSpeed *4) == 0)
                {
                    animationCount = 0;
                    waiting = false;
                }
                animationCount++;
            }
            else if (moving)
            {
                if (animationCount % animationSpeed == 0)
                {
                    if (animationCount % (animationSpeed * 4) == 0)
                    {
                        currentFrame = (currentFrame + 1) % columns;
                        animationCount = 0;
                    }
                    Vector2 change = Vector2.Subtract(movingLocation, location);

                    if (change.X > 0)
                        location.X += moveDistance;
                    else if (change.X < 0)
                        location.X -= moveDistance;
                    if (change.Y > 0)
                        location.Y += moveDistance;
                    else if (change.Y < 0)
                        location.Y -= moveDistance;

                    if (change == Vector2.Zero)
                    {
                        moving = false;
                    }
                }
                animationCount++;
            }
            else if (currentMove != -1)
            {
                DoNextMovement();
            }
            // reset sprite frame to 0th frame when not moving
            else if (currentFrame != 0)
            {
                if (animationCount % (animationSpeed * 4) == 0)
                {
                    if (currentFrame > columns / 2)
                        currentFrame += 1;
                    else
                        currentFrame -= 1;
                    currentFrame = currentFrame % columns;
                    animationCount = 0;
                }
                animationCount++;
            }
        }

        //TODO: This is extremely similar to baseSprite. Maybe can be merged
        public override void Draw(SpriteBatch spriteBatch)
        {
            bounds.CentreAtLocation(location);
            if (!GameOfRPG.Camera.SpriteIsVisible(bounds))
                return;
            int x = currentFrame * bounds.Width;
            int y = (int)direction * bounds.Height;
            Rectangle sourceRectangle = new Rectangle(x, y, bounds.Width, bounds.Height);
            Vector2 drawLocation = GameOfRPG.Camera.GetScreenLocation(bounds.UpperLeft);
            spriteBatch.Draw(texture, drawLocation, null, sourceRectangle, null, 0, null, Color.White, SpriteEffects.None, 0.5f);
            sourceRectangle = new Rectangle(x, y, bounds.Width, bounds.Height - 14);
            spriteBatch.Draw(texture, drawLocation, null, sourceRectangle, null, 0, null, Color.White, SpriteEffects.None, 0.4f);



        }
    }
}

