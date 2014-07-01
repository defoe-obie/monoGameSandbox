using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameSandbox
{
    public class RPGWindow
    {


        //private Texture2D lookAndFeel;
        private Texture2D foreground, background;
        protected SpriteFont font;

        //private Rectangle[] sections;
        private Rectangle[] newSections;
        //private int arrowCount, arrowAnimationCount;

       
        private enum Section
        {
            Background,
            CornerTopLeft,
            CornerTopRight,
            CornerBottomLeft,
            CornerBottomRight,
            SideTop,
            SideLeft,
            SideBottom,
            SideRight,
            Arrow1,
            Arrow2,
            Arrow3
        }

        public RPGWindow()
        {
            //lookAndFeel = Constants.defaultLookAndFeel;
            foreground = Constants.defaultLookAndFeelForeground;
            background = Constants.defaultLookAndFeelBackground;
            font = Constants.defaultFont;

            //arrowCount = 0;
            //            arrowAnimationCount = 0;
            // temporary values for my experiment
//            int ts = Constants.TileSize;
//            List<Rectangle> tempList = new List<Rectangle>();
//            tempList.Add(new Rectangle(ts * 3, 0, ts * 3, ts * 3));
//            tempList.Add(new Rectangle(0, 0, ts, ts));
//            tempList.Add(new Rectangle(ts * 2, 0, ts, ts));
//            tempList.Add(new Rectangle(0, ts * 2, ts, ts));
//            tempList.Add(new Rectangle(ts * 2, ts * 2, ts, ts));
//            tempList.Add(new Rectangle(ts, 0, ts, ts));
//            tempList.Add(new Rectangle(0, ts, ts, ts));
//            tempList.Add(new Rectangle(ts, ts * 2, ts, ts));
//            tempList.Add(new Rectangle(ts * 2, ts, ts, ts));
//            tempList.Add(new Rectangle(0, ts * 3, ts, ts));
//            tempList.Add(new Rectangle(ts, ts * 3, ts, ts));
//            tempList.Add(new Rectangle(ts * 2, ts * 3, ts, ts));
//            sections = tempList.ToArray();
//            tempList.Clear();

            newSections = new Rectangle[15];

            // Corners: TL, TR, BL, BR
            newSections[0] = new Rectangle(0, 0, 10, 10);
            newSections[1] = new Rectangle(30, 0, 10, 10);
            newSections[2] = new Rectangle(0, 30, 10, 10);
            newSections[3] = new Rectangle(30, 30, 10, 10);

            // Sides: T, L, R, B
            newSections[4] = new Rectangle(10, 0, 10, 10);
            newSections[5] = new Rectangle(0, 10, 10, 10);
            newSections[6] = new Rectangle(30, 10, 10, 10);
            newSections[7] = new Rectangle(10, 30, 10, 10);

            // 'T' Sections: T, L, R, B
            newSections[8] = new Rectangle(20, 0, 10, 10);
            newSections[9] = new Rectangle(0, 20, 10, 10);
            newSections[10] = new Rectangle(30, 20, 10, 10);
            newSections[11] = new Rectangle(20, 30, 10, 10);

            // Inner Sections: Down, Across
            newSections[12] = new Rectangle(20, 10, 10, 10);
            newSections[13] = new Rectangle(10, 20, 10, 10);

            // Cross Section: C
            newSections[14] = new Rectangle(20, 20, 10, 10);

        }

        public void ChangeLookAndFeel(Texture2D backgroundLook, Texture2D foregroundLook)
        {
            background = backgroundLook;
            foreground = foregroundLook;
        }

        protected float[] ConvertIntLocationToPercent(int dimensionTotal, params int[] locations)
        {
            float[] result = new float[locations.Length];
            for (int i = 0; i < locations.Length; ++i)
            {
                result[i] = (float)locations[i] / dimensionTotal;
            }
            return result;
        }

        protected void Draw(SpriteBatch sb, Rectangle messageArea, float[] verticalSplits, float[] horizontalSplits, float backgroundAlpha, bool showArrow = false)
        {
            Color backgroundColor = Color.White * backgroundAlpha;
            Color drawColor = Color.White;

            // assigned for convenience in the draw calls
            int t = messageArea.Top;
            int l = messageArea.Left;
            int r = messageArea.Right;
            int b = messageArea.Bottom;
            int w = messageArea.Width;
            int h = messageArea.Height;

            // linearWrap so the background can be tiled
            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, GameOfRPG.Variables.Scale);

            sb.Draw(background, new Vector2(l, t), messageArea, backgroundColor);

            // draw sides: top=4, left=5, right=6, bottom=7
            sb.Draw(foreground, new Rectangle(l, t, w, 10), newSections[4], drawColor);
            sb.Draw(foreground, new Rectangle(l, t, 10, h), newSections[5], drawColor);
            sb.Draw(foreground, new Rectangle(r - 10, t, 10, h), newSections[6], drawColor);
            sb.Draw(foreground, new Rectangle(l, b - 10, w, 10), newSections[7], drawColor);

            // draw corners: topLeft=0, topRight=1, bottomLeft=2, bottomRight=3
            sb.Draw(foreground, new Rectangle(l, t, 10, 10), newSections[0], drawColor);
            sb.Draw(foreground, new Rectangle(r - 10, t, 10, 10), newSections[1], drawColor);
            sb.Draw(foreground, new Rectangle(l, b - 10, 10, 10), newSections[2], drawColor);
            sb.Draw(foreground, new Rectangle(r - 10, b - 10, 10, 10), newSections[3], drawColor);

            // Draw vertical splits
            foreach (float x in verticalSplits)
            {
                sb.Draw(foreground, new Rectangle(l + (int)(w * x) - 5, t, 10, h), newSections[12], drawColor);
                sb.Draw(foreground, new Rectangle(l + (int)(w * x) - 5, t, 10, 10), newSections[8], drawColor);
                sb.Draw(foreground, new Rectangle(l + (int)(w * x) - 5, b - 10, 10, 10), newSections[11], drawColor);
            }
            // Draw horizontal splits and cross sections
            foreach (float y in horizontalSplits)
            {
                sb.Draw(foreground, new Rectangle(l, t + (int)(h * y) - 5, w, 10), newSections[13], drawColor);
                sb.Draw(foreground, new Rectangle(l, t + (int)(h * y) - 5, 10, 10), newSections[9], drawColor);
                sb.Draw(foreground, new Rectangle(r - 10, t + (int)(h * y) - 5, 10, 10), newSections[10], drawColor);
                // Draw cross sections
                foreach (float x in verticalSplits)
                {
                    sb.Draw(foreground, new Rectangle(l + (int)(w * x) - 5, t + (int)(h * y) - 5, 10, 10), newSections[14], drawColor);
                }
            }


            sb.End();
        }


        protected void Draw(SpriteBatch sb, Rectangle messageArea, float backgroundAlpha, bool showArrow = false)
        {
            this.Draw(sb, messageArea, new float[]{ }, new float[]{ }, backgroundAlpha, showArrow);
        }

        protected void Draw(SpriteBatch sb, Rectangle messageArea, bool showArrow = false)
        {
            this.Draw(sb, messageArea, new float[]{ }, new float[]{ }, 1f, showArrow);
        }

        protected void DrawBackground(SpriteBatch sb, Rectangle messageArea, float backgroundAlpha){
            Color backgroundColor = (new Color(0.3f,0.6f,0.55f)) * backgroundAlpha;

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, GameOfRPG.Variables.Scale);
            sb.Draw(background, new Vector2(messageArea.Left, messageArea.Top), messageArea, backgroundColor);
            sb.End();
        }


        protected void DrawWithFancyBorder(SpriteBatch sb, Rectangle messageArea, float backgroundAlpha, bool showArrow = false)
        {
            float[] across = ConvertIntLocationToPercent(messageArea.Width, 11, messageArea.Width - 9);
            float[] down = ConvertIntLocationToPercent(messageArea.Height, 10, messageArea.Height - 10);
            Draw(sb, messageArea, across, down, backgroundAlpha, showArrow);
        }
    }
}

