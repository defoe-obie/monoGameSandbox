using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameSandbox
{
    [Debug]
    public class RPGDebugger : RPGWindow
    {

        private struct RPGDebuggerMessage
        {
            public string text;
            public Color color;

        }

        private List<RPGDebuggerMessage> messages;
        private Vector2 startPosition;
        private float messageScroll;
        private float totalHeight;

        public DebugType ShowingType{ get; set; }

        public enum DebugType
        {
            Messages,
            Variables,
            Switches
        }

        public RPGDebugger() : base()
        {
            startPosition = new Vector2(30, 30);
            messageScroll = 0;
            totalHeight = 0;
            messages = new List<RPGDebuggerMessage>();
            ShowingType = DebugType.Messages;
        }

        /// <summary>
        /// Adds a red error message to the debugger. As opposed to the others, this one signals that the debugger
        /// needs to be shown immediately.
        /// </summary>
        /// <param name="message">The error message</param>
        public void AddError(string message)
        {
            AddMessageWithColor(message, Color.LightCoral);
            //TODO: separate scroll amounts / debugType
            messageScroll = 0;
            GameOfRPG.Variables.DebuggerWaiting = true;
        }

        public void AddWarning(string message)
        {
            AddMessageWithColor(message, Color.Yellow);
        }

        public void AddMessage(string message)
        {
            AddMessageWithColor(message, Color.White);
        }


        public void AddMessageWithColor(string message, Color color)
        {
            RPGDebuggerMessage dm;
            dm.text = message + GameOfRPG.Camera.DebugSnapShot();
            dm.color = color;
            messages.Add(dm);
            if (messages.Count > 30){
                messages.RemoveAt(0);
            }
        }


        public void Update(KeyboardState ks, KeyboardState lastState)
        {
            if (ks.IsKeyDown(Keys.Left) && lastState.IsKeyUp(Keys.Left)){

                if (ShowingType != DebugType.Messages)
                {
                    ShowingType = (DebugType)((int)ShowingType - 1);
                    messageScroll = 0;
                }
            }
            else if (ks.IsKeyDown(Keys.Right) && lastState.IsKeyUp(Keys.Right)){
                if (ShowingType != DebugType.Switches)
                {
                    ShowingType = (DebugType)((int)ShowingType + 1);
                    messageScroll = 0;
                }
            }
            //tongue twister variable names
            bool isKeyUpPressed = ks.IsKeyDown(Keys.Up);
            bool isKeyDownPressed = ks.IsKeyDown(Keys.Down);
            if (isKeyUpPressed && isKeyDownPressed)
                return;
            if (isKeyDownPressed)
            {
                if (totalHeight > Constants.ScreenHeight - 60)
                    messageScroll = Math.Max(-totalHeight + Constants.ScreenHeight - 60, messageScroll - 5f);
            }
            else if (isKeyUpPressed)
            {
                if (messageScroll < 0)
                    messageScroll = messageScroll + 5f;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle messageArea = new Rectangle(0, 0, Constants.PlayAreaWidth, Constants.ScreenHeight);
            base.DrawWithFancyBorder(sb, messageArea, 0.9f);
            totalHeight = 0f;

            sb.Begin();
            sb.DrawString(font, ShowingType.ToString(), Vector2.Zero, Color.White);
            sb.End();

            startPosition = new Vector2(30, messageScroll + 30);
            Rectangle destinationArea = new Rectangle(30, 30, messageArea.Width, messageArea.Height-60);
            RasterizerState rs = new RasterizerState(){ ScissorTestEnable = true };
            //sb.Begin(SpriteSortMode.Deferred, null, null, null, rs, null, null);
            sb.Begin(SpriteSortMode.Deferred, null, null, null, rs);
            sb.GraphicsDevice.ScissorRectangle = destinationArea;
            if(ShowingType == DebugType.Messages){
                messages.Reverse();
                foreach (RPGDebuggerMessage rdm in messages)
                {
                    sb.DrawString(font, rdm.text, startPosition, rdm.color);
                    totalHeight += font.MeasureString(rdm.text).Y + 10;
                    startPosition.Y += font.MeasureString(rdm.text).Y + 10;
                }
                messages.Reverse();
            }
            else if (ShowingType == DebugType.Variables){
                foreach (string s in GameOfRPG.Variables.GetDebugList()){
                    sb.DrawString(font, s, startPosition, Color.White);
                    totalHeight += font.MeasureString(s).Y + 10;
                    startPosition.Y += font.MeasureString(s).Y + 10;
                }
            }
            else if (ShowingType == DebugType.Switches){
                foreach (string s in GameOfRPG.Variables.GetSwitchList()){
                    sb.DrawString(font, s, startPosition, Color.White);
                    totalHeight += font.MeasureString(s).Y + 10;
                    startPosition.Y += font.MeasureString(s).Y + 10;
                }
            }

            sb.End();

        }
    }
}

