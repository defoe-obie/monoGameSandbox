using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameSandbox
{
    public class MapEffects
    {
        private Color tintColor;
        private float darkness;
        private List<Light> lights;


        private static Texture2D whitePixel;
        public static void SetTextures(Texture2D whitepixel){
            whitePixel = whitepixel;
        }

        public MapEffects()
        {
            tintColor = new Color(128, 28, 128, 64);
            darkness = 0.2f;

        }

        public void ClearLights(){
            lights.Clear();
        }

        public void AddLights(params Light [] newLights){
            lights.AddRange(newLights);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, GameOfRPG.Variables.Scale);
            sb.Draw(whitePixel, Vector2.Zero, null, null, null, 0, new Vector2(Constants.PlayAreaWidth, Constants.ScreenHeight), Color.Black * darkness, SpriteEffects.None, 0);
            //sb.Begin();
            sb.Draw(whitePixel, Vector2.Zero, null, null, null, 0, new Vector2(Constants.PlayAreaWidth, Constants.ScreenHeight), tintColor * (tintColor.A / 255f), SpriteEffects.None, 0);
            //foreach(Light l in lights){
                //l.Draw(sb);
                //}
            //for(int y = 0; y < Constants.ScreenTilesDown; ++y){
            //  sb.Draw(whitePixel, new Vector2(0, 28 * y + 14), null, null, null, 0, new Vector2(Constants.ScreenWidth, 14), Color.White, SpriteEffects.None, 1);
            //}
            sb.End();

        }
    }
}

