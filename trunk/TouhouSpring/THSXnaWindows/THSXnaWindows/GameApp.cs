using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaGame = Microsoft.Xna.Framework.Game;

namespace TouhouSpring
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class GameApp : XnaGame
	{
		private GraphicsDeviceManager m_graphics;
		
		public GameApp()
		{
			m_graphics = new GraphicsDeviceManager(this);
			m_graphics.PreferredBackBufferWidth = 1024;
			m_graphics.PreferredBackBufferHeight = 768;
			m_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
			m_graphics.PreferMultiSampling = true;

			Content.RootDirectory = "Content";
			Window.Title = "TouhouSpring";
			IsMouseVisible = true;
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

			GameApp.ServiceContainer.Traverse(TouhouSpring.Services.UpdateDependencyAttribute.Category, false, srv => srv.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f));

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

		// statics
		public static XnaGame Instance
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
