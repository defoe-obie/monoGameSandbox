#region File Description
//-----------------------------------------------------------------------------
// MonoGameSandboxGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;


#endregion
namespace MonoGameSandbox
{
    /// <summary>
    /// Default Project Template
    /// </summary>
    public class GameOfRPG : Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RPGMovingSprite testsprite, ts2, ts3, ts4, ts5;
        RPGPlayerSprite player;
        GameMap testmap;
        KeyboardState previousKs, currentKs;

        public static FieldOfView Camera;
        public static Random randomGenerator;
        public static RPGMessage Message;
        public static GameVariables Variables;
        public static MessageBox Box;

        [Debug]
        public static RPGDebugger Debugger;

        #endregion


        #region Initialization

        public GameOfRPG()
        {

            graphics = new GraphicsDeviceManager(this);
                
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 600;
            //   this.TargetElapsedTime = TimeSpan.FromTicks(160000);
        }

        /// <summary>
        /// Overridden from the base Game.Initialize. Once the GraphicsDevice is setup,
        /// we'll use the viewport to initialize some values.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        [Debug]
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be use to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            randomGenerator = new Random();
            Variables = new GameVariables();

            SpriteFont font = Content.Load<SpriteFont>("DataFont");
            Constants.defaultFont = font;
            Constants.defaultLookAndFeel = Content.Load<Texture2D>("messagebg2");
            Constants.defaultLookAndFeelBackground = Content.Load<Texture2D>("window_bg");
            Constants.defaultLookAndFeelForeground = Content.Load<Texture2D>("window_fg");

            Message = new RPGMessage();
            Box = new MessageBox();
            Debugger = new RPGDebugger();

            Light.SetTexture(Content.Load<Texture2D>("whitepixel"));
            MapEffects.SetTextures(Content.Load<Texture2D>("whitepixel"));
            // effectsRender = new RenderTarget2D(spriteBatch.GraphicsDevice, spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth, spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight);
            #region testsprites
            Texture2D nyrehm = Content.Load<Texture2D>("nyrehm_spearmaster_01");
            Texture2D playa = Content.Load<Texture2D>("manbase");

            testsprite = new RPGMovingSprite("001", nyrehm, 4);
            ts2 = new RPGMovingSprite("002", nyrehm, 4);
            ts3 = new RPGMovingSprite("003", nyrehm, 4);
            ts4 = new RPGMovingSprite("004", nyrehm, 4);
            ts5 = new RPGMovingSprite("005", nyrehm, 4);
            
            testsprite.SetAnimationProperties(true, 1, 0, RPGBaseSprite.Speed.VeryFast);
            ts2.SetAnimationProperties(true, 1, 0, RPGBaseSprite.Speed.Fast);
            ts3.SetAnimationProperties(true, 1, 0, RPGBaseSprite.Speed.Normal);
            ts4.SetAnimationProperties(true, 1, 0, RPGBaseSprite.Speed.Slow);
            ts5.SetAnimationProperties(true, 1, 0, RPGBaseSprite.Speed.VerySlow);
            
            testsprite.SetLocationByTile(10, 10);
            ts2.SetLocationByTile(11, 10);
            ts3.SetLocationByTile(12, 10);
            ts4.SetLocationByTile(13, 10);
            ts5.SetLocationByTile(14, 10);
            
            testsprite.SetMovementPattern(true, 10);
            ts2.SetMovementPattern(true, 10);
            ts3.SetMovementPattern(true, 10);
            ts4.SetMovementPattern(true, 10);
            ts5.SetMovementPattern(true, 10);
            #endregion

            player = new RPGPlayerSprite("Player", nyrehm, 4);
              
            Camera = new FieldOfView();
            testmap = Data.Maps.LoadMap(Content, 0);
            Camera.InitializeToMap(testmap);
            testmap.SetUpLights(graphics);
            //InnerException  {System.IO.DirectoryNotFoundException: Could not find a part of the path "/Users/lauraabresch/This/Programming/C# Workspace/MonoGameSandbox/MonoGameSandbox/bin/Debug/MonoGameSandbox.app/GameOfRPG/Data/GameData/tiles.rpgdata".   at System.IO.FileStream..ctor (System.String path, FileMode mode, FileAccess access, FileShare share, Int32 bufferSize, Boolean anonymous, FileOptions options) [0x00000] in <filename unknown>:0    at System.IO.FileStream..ctor (System.String path, FileMode mode, FileAccess access, FileShare share) [0x00000] in <filename unknown>:0    at (wrapper remoting-invoke-with-check) System.IO.FileStream:.ctor (string,System.IO.FileMode,System.IO.FileAccess,System.IO.FileShare)   at System.IO.File.OpenRead (System.String path) [0x00000] in <filename unknown>:0    at System.IO.StreamReader..ctor (System.String path, System.Text.Encoding encoding, Boolean detectEncodingFromByteOrderMarks, Int32 bufferSize) [0x00000] in <filename unknown>:0    at System.IO.StreamReader..ctor (System.String path) [0x00000] in <filename unknown>:0    at (wrapper remoting-invoke-with-check) System.IO.StreamReader:.ctor (string)   at MonoGameSandbox.TileData.LoadData () [0x00012] in /Users/lauraabresch/This/Programming/C# Workspace/MonoGameSandbox/MonoGameSandbox/GameOfRPG/Data/TileData.cs:55    at MonoGameSandbox.Data..cctor () [0x0005f] in /Users/lauraabresch/This/Programming/C# Workspace/MonoGameSandbox/MonoGameSandbox/GameOfRPG/Data/Data.cs:24 }  System.IO.DirectoryNotFoundException
            Variables.SetScale(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            previousKs = Keyboard.GetState();
        }



        #endregion
        //Could not find a part of the path "/Users/lauraabresch/This/Programming/C# Workspace/MonoGameSandbox/MonoGameSandbox/bin/Debug/MonoGameSandbox.app/GameOfRPG/Data/GameData/tiles.rpgdata".  
        #region Update and Draw

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        [Debug]
        protected override void Update(GameTime gameTime)
        {

            currentKs = Keyboard.GetState();

            // TODO: remove this :D
            if (currentKs.IsKeyDown(Keys.Escape))
                this.Exit();

            #region gamedebugger
            // Begin by checking the Debugger State
            if (currentKs.IsKeyDown(Keys.D) && previousKs.IsKeyUp(Keys.D)){
                Variables.DebuggerShowing = !Variables.DebuggerShowing;
            }
            if(Variables.DebuggerWaiting){
                Variables.DebuggerWaiting = false;
                Variables.DebuggerShowing = true;
            }

            // Don't update anything else if the debugger is showing
            // ie. Pause the game.
            if( Variables.DebuggerShowing){

                Debugger.Update(currentKs, previousKs);
                base.Update(gameTime);
                previousKs = currentKs;
                return;
            }
            #endregion


            // Check if waiting for input on a currently displayed message.
            if (Variables.MessageShowing && currentKs.IsKeyDown(Keys.Enter) && previousKs.IsKeyUp(Keys.Enter))
            {
                // PrepMessage handles MessageShowing/MessageWaiting flags, and checks for undisplayed MessageText
                Message.PrepMessage();
            }

            #region temp
            //TODO: More temp stuff
            testsprite.Update();
            ts2.Update();
            ts3.Update();
            ts4.Update();
            ts5.Update();
            #endregion

            // Player and interpreter do not run while a message is waiting.
            if (!Variables.MessageWaiting)
            {

                // The player does not update when the interpreter is busy.
                if (!Event.Interpreter.Busy)
                {
                    if (currentKs.IsKeyDown(Keys.LeftShift))
                    {
                        Box.Update(currentKs);
                    }
                    else
                    {
                        player.Update(currentKs, previousKs);
                    }
                }
                Event.Interpreter.UpdateInterpreter();
            }

            // Update Camera View based on player location
            Camera.Update(player.Location);

            // TODO: All the sprites for the map will be updated through GameMap
            // Not Implemented Yet.

            base.Update(gameTime);
            previousKs = currentKs;
        }

        /// <summary>
        /// This is called when the game should draw itself. 
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        [Debug]
        protected override void Draw(GameTime gameTime)
        {
            // Clear the backbuffer
            graphics.GraphicsDevice.Clear(Color.Black);
            // Draw the map, and the player, and eventually also all the sprites.
            Camera.Draw(spriteBatch, player);

            // Draw the in-game debugger, if it is showing.
            if(Variables.DebuggerShowing){
                Debugger.Draw(spriteBatch);
                base.Draw(gameTime);
                return;
            }

            // If a message is waiting, draw the message
            if (Variables.MessageWaiting)
            {
                Message.Draw(spriteBatch);
            }

            Box.Draw(spriteBatch);

            base.Draw(gameTime);

        }



        #endregion
    }
}
