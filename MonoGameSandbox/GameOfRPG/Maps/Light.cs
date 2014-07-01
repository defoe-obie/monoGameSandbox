using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGameSandbox
{
    public class Light
    {
        private static Texture2D pixel;

        private Vector2 location;
        private float rotation;
        private int radius;
        private RenderTarget2D render;


        public Light(GraphicsDeviceManager graphics, int tilex, int tiley, Vector2 direction, float scopeAngle, float strength, float tileRadius, Color color)
        {
            this.location = new Vector2(tilex * Constants.TileSize, tiley * Constants.TileSize);
            this.location = Vector2.Add(this.location, new Vector2(14, 14));
            float realStrength = strength / 3;
            this.radius = (int)(tileRadius * Constants.TileSize);

            Vector2 d = Vector2.Normalize(direction);
            this.rotation = (float)Math.Acos((double)(d.X));
            if (direction.Y < 0)
                rotation = -rotation;


            List<Vector2> visiblePoints = new List<Vector2>();
            List<float> strengthAtPoints = new List<float>();
            double minXdirection = Math.Cos(scopeAngle / 2);
            double minYdirection = -Math.Sqrt(1 - (minXdirection * minXdirection));
            Vector2 minDirection = new Vector2((float)minXdirection, (float)minYdirection);
            Vector2 maxDirection = minDirection;
            maxDirection.Y = -maxDirection.Y;
            for(int i = 0; i < radius; ++i){
                Vector2 newVector = Vector2.Multiply(minDirection, i);
                newVector.X = (int)newVector.X;
                newVector.Y = (int)newVector.Y;
                if (!visiblePoints.Contains(newVector)){
                    visiblePoints.Add(newVector);
                    if (newVector.Y != 0)
                    {
                        newVector.Y = -newVector.Y;
                        visiblePoints.Add(newVector);
                    }
                }

            }
            List<Vector2> newpoints = new List<Vector2>();
            foreach(Vector2 v in visiblePoints){
                for( int i = (int)v.X; i < radius; ++i){
                    Vector2 newone = new Vector2(i + 1, v.Y);
                    if (!newpoints.Contains(newone))
                    {
                        if (!visiblePoints.Contains(newone))
                         {
                            newpoints.Add(newone);
                         }
                    }
                }
            }
            visiblePoints.AddRange(newpoints);
            int radiusSquared = radius * radius;
            float ratio = realStrength / radiusSquared;
            for(int i = 0; i < visiblePoints.Count; ++i){
                Vector2 v = visiblePoints[i];
                float distance = Vector2.Dot(v, v);
                if (distance > radiusSquared)
                    strengthAtPoints.Add(0);
                else
                    strengthAtPoints.Add((radiusSquared - distance) * ratio);
                visiblePoints[i] = Vector2.Add(v, new Vector2(0, radius));

            }
            render = new RenderTarget2D(graphics.GraphicsDevice, radius , radius * 2 );
            graphics.GraphicsDevice.SetRenderTarget(render);
            graphics.GraphicsDevice.Clear(Color.Transparent);
            SpriteBatch sb = new SpriteBatch(graphics.GraphicsDevice);
            sb.Begin();
            for(int i = 0; i < visiblePoints.Count; ++i){
                sb.Draw(pixel, visiblePoints[i], color * strengthAtPoints[i]);
            }
            //   sb.Draw(pixel, visiblePoints[0], Color.Black);
            sb.End();
            graphics.GraphicsDevice.SetRenderTarget(null);
        }

        public static void SetTexture(Texture2D pixeltexture){
            pixel = pixeltexture;
        }

        public void Draw(SpriteBatch sb){
            sb.Draw(render, GameOfRPG.Camera.GetScreenLocation(location), null, null, new Vector2(0, radius),rotation,null,Color.White,SpriteEffects.None, 0);
        }
    }
}

