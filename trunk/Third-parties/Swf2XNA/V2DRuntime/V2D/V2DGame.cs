using System;
using System.Collections.Generic;
using DDW.Display;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DDW.V2D 
{
    public abstract class V2DGame : Microsoft.Xna.Framework.Game
    {
        public static V2DGame instance;
        public static ContentManager contentManager;
        public const string ROOT_NAME = V2DWorld.ROOT_NAME;
        public static string currentRootName = V2DWorld.ROOT_NAME;

        public static PlayerIndex activeController = PlayerIndex.One;

        protected GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        protected bool keyDown = false;
        protected bool isFullScreen = false;

        protected V2DGame()
        {
            if (instance != null)
            {
                throw new Exception("There can be only one game class.");
            }

            instance = this;
            graphics = new GraphicsDeviceManager(this);
            contentManager = Content;
            Content.RootDirectory = "Content";
        }

        protected virtual void CreateScreens()
        {
            //screenPaths.Add(symbolImports[i]);
        }
        public virtual void AddingScreen(Screen screen) { }
        public virtual void RemovingScreen(Screen screen) { }
        protected override void Initialize()
        {
            base.Initialize();

            CreateScreens();
        }

        public void SetSize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                graphics.SynchronizeWithVerticalRetrace = true;
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;
                graphics.IsFullScreen = this.isFullScreen;
                graphics.ApplyChanges();
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void UnloadContent()
        {
        }
        public virtual void ExitGame()
        {
            this.Exit();
        }

    }
}
