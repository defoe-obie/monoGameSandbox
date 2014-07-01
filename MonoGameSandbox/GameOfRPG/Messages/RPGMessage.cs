using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MonoGameSandbox
{
    public class RPGMessage : RPGWindow
    {
        //private Texture2D lookAndFeel;
        //private SpriteFont font;
        //private Rectangle[] sections;
        private string currentMessage;
        private int currentLetter;

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
            SideRight
        }

       
      
        public RPGMessage() : base()
        {
            currentMessage = "";
            currentLetter = 0;
        }

        /// <summary>
        /// Split a waiting message into lines based on length, and
        /// sets the flag for a waiting message, if the message text is not empty.
        /// </summary>
        public void PrepMessage()
        {
            GameOfRPG.Variables.MessageShowing = false;
            string text = GameOfRPG.Variables.MessageText;
            if (text == "")
            {
                GameOfRPG.Variables.MessageWaiting = false;
                return;
            }

            string[] words = text.Split(' ');
            List<string> lines = new List<string>();

            int wordcount = 0;

            while (lines.Count < 4)
            {
                string line = "";
                Vector2 messagesize;
                do
                {
                    if (wordcount == words.Length)
                        break;

                    string word = words[wordcount];
                    if (word == "")
                    {
                        wordcount++;
                        continue;
                    }
                    if (word.Contains("@["))
                    {
                        if (word.StartsWith("@["))
                        {
                            // @[v#] --> game Variable[#]
                            if (word.StartsWith("@[v"))
                            {
                                string[] newLineSplit = word.Substring(3).Split(new char[]{ ']' }, 2);
                                string variable = GameOfRPG.Variables[Convert.ToInt32(newLineSplit[0])].ToString();
                                line = String.Join("", line, variable);
                                words[wordcount] = newLineSplit[1];
                            }
                        }
                        else
                        {
                            string[] newLineSplit = word.Split(new char[]{ '@' }, 2);
                            line = String.Join(" ", line, newLineSplit[0]);
                            words[wordcount] = '@' + newLineSplit[1];
                        }
                        continue;
                    }
                    if (word.Contains("\n"))
                    {
                        string[] newLineSplit = word.Split(new char[]{ '\n' }, 2);
                        line = String.Join(" ", line, newLineSplit[0]);
                        words[wordcount] = newLineSplit[1];
                        break;

                    }

                    line = String.Join(" ", line, words[wordcount]);
                    messagesize = font.MeasureString(line);
                    wordcount++;

                }
                while (messagesize.X < 200);
                //TODO: Doing this means that one cannot specifically put in spaces
                // That may not be a good plan, you know!
                lines.Add(line.Trim());

            }
            currentMessage = String.Join("\n", lines.ToArray());

            currentLetter = 0;

            GameOfRPG.Variables.MessageText = String.Join(" ", words, wordcount, words.Length - wordcount);
            GameOfRPG.Variables.MessageWaiting = true;
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle messageArea = new Rectangle(0, Constants.ScreenHeight - 140, Constants.PlayAreaWidth, 140);
            base.DrawWithFancyBorder(sb, messageArea, 0.9f, true);

            // draw message one letter at a time
            sb.Begin(SpriteSortMode.Immediate, null, null, null, null, null, GameOfRPG.Variables.Scale);

            Vector2 textlocation = new Vector2(30, messageArea.Top + 25);
            sb.DrawString(font, currentMessage.Substring(0, currentLetter), textlocation, Color.White);

            sb.End();

            // complete message drawn
            if (currentLetter == currentMessage.Length)
            {
                GameOfRPG.Variables.MessageShowing = true;
            }
            else
            {
                currentLetter += 1;
            }
           
                
        }
    }
}

