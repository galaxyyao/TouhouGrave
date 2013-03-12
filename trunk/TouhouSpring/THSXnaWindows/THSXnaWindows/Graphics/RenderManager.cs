using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
	partial class RenderManager : Services.GameService
	{
		public GraphicsDevice Device
		{
			get { return GameApp.Instance.GraphicsDevice; }
		}

		public ContentManager Content
		{
			get { return GameApp.Instance.Content; }
		}

		public override void Startup()
		{
			CreateQuadRenderer();
		}

		public override void Shutdown()
		{
			DestroyQuadRenderer();
		}
	}
}
