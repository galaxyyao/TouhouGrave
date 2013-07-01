using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TouhouSpring
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameApp : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager m_graphics;
        private Dictionary<string, string> m_commandlineArgs = new Dictionary<string, string>();

        public MouseState MouseState
        {
            get; private set;
        }

        public KeyboardState KeyboardState
        {
            get; private set;
        }

        public int MouseY
        {
            get; private set;
        }

        public GameApp()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_graphics.PreferredBackBufferWidth = 1024;
            m_graphics.PreferredBackBufferHeight = 768;
            m_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            m_graphics.PreferMultiSampling = true;

            m_graphics.DeviceResetting += new EventHandler<EventArgs>(DeviceResetting);
            m_graphics.DeviceReset += new EventHandler<EventArgs>(DeviceReset);

            Content.RootDirectory = "Content";
            Window.Title = "TouhouSpring";
            IsMouseVisible = true;

            foreach (var arg in Environment.GetCommandLineArgs().Skip(1))
            {
                var argStr = arg;
                if (argStr.StartsWith("--"))
                {
                    argStr = argStr.Substring(2);
                }
                else if (arg.StartsWith("-"))
                {
                    argStr = argStr.Substring(1);
                }

                int delimiter = argStr.IndexOfAny(new char[] { '=', ':' });
                if (delimiter == -1)
                {
                    m_commandlineArgs.Add(argStr, String.Empty);
                }
                else
                {
                    m_commandlineArgs.Add(argStr.Substring(0, delimiter), argStr.Substring(delimiter + 1));
                }
            }
        }

        public string GetCommandLineArgValue(string arg)
        {
            string value;
            return m_commandlineArgs.TryGetValue(arg, out value) ? value : null;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void BeginRun()
        {
            Instance = this;
            ServiceContainer = new Services.Container<Services.GameService>();
            ServiceContainer.Startup();

            base.BeginRun();

#if WINDOWS
            // set the existing threads' affinity to the first core
            if (Environment.ProcessorCount > 1)
            {
                var process = Process.GetCurrentProcess();
                foreach (ProcessThread thread in process.Threads)
                {
                    thread.ProcessorAffinity = (IntPtr)1;
                }
            }
#endif
        }

        protected override void EndRun()
        {
            base.EndRun();

            ServiceContainer.Shutdown();
            ServiceContainer = null;
            Instance = null;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();
            GameApp.ServiceContainer.Traverse(TouhouSpring.Services.UpdateDependencyAttribute.Category, false, srv => srv.Update((float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f)));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GameApp.ServiceContainer.Traverse(TouhouSpring.Services.RenderDependencyAttribute.Category, false, srv => srv.Render());

            base.Draw(gameTime);
        }

        private void DeviceResetting(object sender, EventArgs e)
        {
            GameApp.ServiceContainer.Traverse(TouhouSpring.Services.PreDeviceResetDependencyAttribute.Category, false, srv => srv.PreDeviceReset());
        }

        private void DeviceReset(object sender, EventArgs e)
        {
            GameApp.ServiceContainer.Traverse(TouhouSpring.Services.PostDeviceResetDependencyAttribute.Category, false, srv => srv.PostDeviceReset());
        }

        // statics
        public static GameApp Instance
        {
            get; private set;
        }

        public static Services.Container<Services.GameService> ServiceContainer
        {
            get; private set;
        }

        public static T Service<T>() where T : Services.GameService
        {
            if (ServiceContainer == null)
            {
                throw new InvalidOperationException("ServiceContainer is not created yet.");
            }

            return ServiceContainer.Get<T>();
        }
    }
}
