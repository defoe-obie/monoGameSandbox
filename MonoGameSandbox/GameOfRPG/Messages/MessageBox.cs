using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox
{
    public class MessageBox : RPGWindow
    {
        private struct Message
        {
            public string text;
            public Color textColor;
        }

        private List<Message> messages;
        private int scrollAmount;
        private float totalMessageHeight;
        private const int MAX_MESSAGES = 20;
        private const int MAX_WIDTH = 200;
        private const int MESSAGEAREA_HEIGHT = 280;

        public MessageBox() : base()
        {
            messages = new List<Message>();
            scrollAmount = 0;
            totalMessageHeight = -1f * MESSAGEAREA_HEIGHT;
        }

        public void Update(KeyboardState currentKs)
        {
            if (currentKs.IsKeyDown(Keys.M))
            {
                int r = GameOfRPG.randomGenerator.Next(4);
                string message = "";
                switch (r)
                {
                    case(0):
                        message = "What a waste of space!";
                        break;
                    case(1):
                        message = "This is a generic second message.";
                        break;
                    case(2):
                        message = "This message has two lines to it! Isn't that crazy!";
                        break;
                    case(3):
                        message = "Forecast";
                        break;
                }
                //if (messages.Count < 20)
                //{
                r = GameOfRPG.randomGenerator.Next(6);
                if (r == 0)
                    AddMessage(message + "Floralwhite", Color.FloralWhite);
                else if (r == 1)
                    AddMessage(message + "antiquewhite", Color.AntiqueWhite);
                else if (r == 2)
                    AddMessage(message + "ghostwhite", Color.GhostWhite);
                else if (r == 3)
                    AddMessage(message + "navajowhite", Color.NavajoWhite);
                else if (r == 4)
                    AddMessage(message + "whitesmoke", Color.WhiteSmoke);
                else if (r == 5)
                    AddMessage(message + "white", Color.White);
                if (messages.Count > 20)
                {
                    totalMessageHeight -= (font.MeasureString(messages[0].text).Y + 4);
                    messages.RemoveAt(0);
                }
                //messages.Add(new Message(){ text = message, textColor = Color.White });
                //}
            }
            else if (currentKs.IsKeyDown(Keys.Down))
            {
                //if (totalMessageHeight > 0 && scrollAmount < totalMessageHeight)
                if (scrollAmount < totalMessageHeight)
                {
                    scrollAmount = Math.Min(scrollAmount + 3, (int)totalMessageHeight);
                }
            }
            else if (currentKs.IsKeyDown(Keys.Up))
            {
                if (scrollAmount > 0)
                {
                    scrollAmount = Math.Max(scrollAmount - 3, 0);
                }
            }
            else if (currentKs.IsKeyDown(Keys.Left)){
                scrollAmount = 0;
            }
            else if (currentKs.IsKeyDown(Keys.Right)){
                scrollAmount = (int)totalMessageHeight;
            }

        }

        public void AddMessage(string messageText, Color messageColor)
        {
            string[] words = messageText.Split();
            string substring = "";
            float width = 0;
            Message m = new Message();
            m.textColor = messageColor;
            foreach (string w in words)
            {
                if (width + font.MeasureString(w).X > MAX_WIDTH)
                {
                    m.text += "\n" + substring;
                    width = 0;
                    substring = "";
                        
                }
                substring = string.Join(" ", substring, w);
                width = font.MeasureString(substring).X;

            }
            m.text += "\n" + substring;
            m.text = m.text.Substring(1);
            messages.Add(m);
            totalMessageHeight += font.MeasureString(m.text).Y + 6;
            scrollAmount = 0;
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle messageArea2 = new Rectangle(Constants.PlayAreaWidth, 0, Constants.MessageAreaWidth, Constants.ScreenHeight);
            //            base.Draw(sb, messageArea);
            Rectangle messageArea = new Rectangle(0, 0, Constants.ScreenWidth, Constants.ScreenHeight);
            float x_marker = (Constants.PlayAreaWidth - 1f) / Constants.ScreenWidth;
            base.DrawBackground(sb, messageArea2, 1f);
            base.Draw(sb, messageArea, new float[]{ x_marker }, new float[]{ }, 0f);


            sb.Begin(SpriteSortMode.Immediate, null, null, null, null, null, GameOfRPG.Variables.Scale);

            Vector2 textlocation = new Vector2(messageArea2.Left + 10, messageArea.Top + 10);
            sb.DrawString(font, "Message Box", textlocation, Color.WhiteSmoke);
            textlocation.Y += 24;
            sb.DrawString(font, "---", textlocation, Color.WhiteSmoke);
            textlocation.Y += 24;
            sb.End();

            RasterizerState rs = new RasterizerState(){ ScissorTestEnable = true };

            // manually scale the destination rectangle...
            Vector2 scaledLocation = Vector2.Transform(textlocation, GameOfRPG.Variables.Scale);
            Vector2 scaledwidthheight = new Vector2(MAX_WIDTH, MESSAGEAREA_HEIGHT);
            scaledwidthheight = Vector2.Transform(scaledwidthheight, GameOfRPG.Variables.Scale);
            Rectangle destinationArea = new Rectangle((int)scaledLocation.X, (int)scaledLocation.Y, (int)scaledwidthheight.X, (int)scaledwidthheight.Y);

            sb.Begin(SpriteSortMode.Deferred, null, null, null, rs, null, GameOfRPG.Variables.Scale);
            sb.GraphicsDevice.ScissorRectangle = destinationArea;

            font.LineSpacing = 14;
            textlocation.Y -= scrollAmount;

            for (int i = messages.Count - 1; i >= 0; --i)
            {
                //Console.WriteLine("scrollamount " + scrollAmount + " textlocation " + textlocation);
                sb.DrawString(font, messages[i].text, textlocation, messages[i].textColor);
                textlocation.Y += font.MeasureString(messages[i].text).Y + 6;

            } 
            //Console.WriteLine("height is " + textlocation.Y + " totalHeight is " + totalMessageHeight);
            sb.End();

        }
    }
}

