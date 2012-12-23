using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.Particle
{
	partial class Canvas : GraphicsDeviceControl
	{
		private Stopwatch m_timer;
		private ParticleSorter m_sorter = new ParticleSorter();

		public XnaColor ClearColor
		{
			get; set;
		}

        public ParticleSystemInstance SystemInstance
        {
            get; set;
        }

		public bool KeepUpdating
		{
			get; set;
		}

		protected override void Initialize()
		{
			Initialize_Render();
			Initialize_Camera();
			Initialize_Grid();

            MouseDown += (s, e) => OnMouseDown_Camera(s, e);
            MouseUp += (s, e) => OnMouseUp_Camera(s, e);
            MouseMove += (s, e) => OnMouseMove_Camera(s, e);
            MouseWheel += (s, e) => OnMouseWheel_Camera(s, e);

			m_timer = new Stopwatch();
			m_timer.Start();

			ClearColor = XnaColor.DimGray;
			KeepUpdating = true;

			Application.Idle += (s, e) => { Invalidate(); };
			ResourceLoader.Instance = new MyResourceLoader(Services);
		}

		protected override void Draw()
		{
			GraphicsDevice.Clear(ClearColor);
			UpdateViewMatrix();
			UpdateProjectionMatrix();

			float deltaTime = (float)m_timer.Elapsed.TotalSeconds;
			m_timer.Restart();

			if (deltaTime > 1)
				deltaTime = 1;

			Render_Grid();

			if (SystemInstance != null)
			{
				if (KeepUpdating)
				{
					SystemInstance.Update(deltaTime);
					// TODO: get the cam dir from view matrix
                    m_sorter.Sort(SystemInstance, -Microsoft.Xna.Framework.Vector3.UnitZ);
				}
				Render();
			}
		}
	}
}
