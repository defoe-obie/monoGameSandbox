using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox
{
    public class RPGPlayerSprite : RPGMovingSprite
    {
        private bool triggersChecked;

        public Vector2 MapLocation
        { 
            get
            {
                if (movingLocation.Y < location.Y || movingLocation.X < location.X)
                    return previousLocation;
                return movingLocation;
        
            }
        }

        public RPGPlayerSprite(string name, Texture2D texture, int columns) : base(name, texture, columns)
        {
            animationSpeed = (int)Speed.Fast;
            triggersChecked = false;
        }


        public void Update(KeyboardState ks, KeyboardState lastks)
        {
            if (!moving && !waiting && ks.IsKeyDown(Keys.Enter) && lastks.IsKeyUp(Keys.Enter)){
                GameOfRPG.Camera.CheckTriggers(GetNextLocation(direction), TriggerType.Query);
                GameOfRPG.Camera.CheckTriggers(location, TriggerType.Query, TriggerType.Touch);
            }
            bool down = ks.IsKeyDown(Keys.Down);
            bool left = ks.IsKeyDown(Keys.Left);
            bool up = ks.IsKeyDown(Keys.Up);
            bool right = ks.IsKeyDown(Keys.Right);
            if (down && !up)
                MoveDirection(RPGDirection.Down);
            else if (up && !down)
                MoveDirection(RPGDirection.Up);
            if (left && !right)
                MoveDirection(RPGDirection.Left);
            else if (right && !left)
                MoveDirection(RPGDirection.Right);
            base.Update();

            //if (waiting){
                // }

            if (moving){
                triggersChecked = false;
            }

            if (!moving && !waiting && !triggersChecked){
                GameOfRPG.Camera.CheckTriggers(location, TriggerType.Touch);
                triggersChecked = true;
            }
           
        }
    }
}

