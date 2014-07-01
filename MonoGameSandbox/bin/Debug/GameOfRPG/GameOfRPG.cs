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

        bool messagewaiting;

        public static FieldOfView Camera;
        public static Random randomGenerator;
        public static RPGMessage Message;
        #endregion

        #region Constants

        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;
        public const int TileSize = 20;

        #endregion

        #region Initialization

        public GameOfRPG()
        {

            graphics = new GraphicsDeviceManager(this);
                
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
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
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be use to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            randomGenerator = new Random();
            SpriteFont gameFont = Content.Load<SpriteFont>("Arial");
            Texture2D messagebg = Content.Load<Texture2D>("messagebg");
            Message = new RPGMessage(messagebg, gameFont);
            messagewaiting = false;
            Texture2D nyrehm = Content.Load<Texture2D>("nyrehm_spearmaster_01");
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
            
            player = new RPGPlayerSprite("Player", nyrehm, 4);
              
            Camera = new FieldOfView();
            //Texture2D temptexture = Content.Load<Texture2D>("SideScrollerBlocks/block001");
            testmap = new GameMap(0, "firstmap");
            testmap.SetupTestMap(Content);
            Camera.InitializeToMap(testmap);
                
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();
            if (ks.IsKeyDown(Keys.M))
            {
                messagewaiting = true;
            }
           
            if (messagewaiting && ks.IsKeyDown(Keys.Enter))
            {
                messagewaiting = false;
            }
            testsprite.Update();
            ts2.Update();
            ts3.Update();
            ts4.Update();
            ts5.Update();
            if(!messagewaiting)
                player.Update(ks);
            Camera.Update(player.Location);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself. 
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            // Clear the backbuffer
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            testmap.Draw(spriteBatch, player);
            if (messagewaiting)
            {
                Message.DrawMessage(spriteBatch, "This is a test messs\n age testing. How these. \n. Things work.");
            }
//            testsprite.Draw(spriteBatch);
//            ts2.Draw(spriteBatch);
//            ts3.Draw(spriteBatch);
//            ts4.Draw(spriteBatch);
//            ts5.Draw(spriteBatch);
//            
           // player.Draw(spriteBatch);
            base.Draw(gameTime);
        }

        #endregion
    }
}
