using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Services
{
	partial class GameUI
	{
		private class ZoomBackground : UI.EventDispatcher, UI.IRenderable,
			UI.IEventListener<UI.MouseMoveEventArgs>,
			UI.IEventListener<UI.MouseButton1EventArgs>,
			UI.IEventListener<UI.MouseButton2EventArgs>
		{
			private Matrix m_toScreenSpace;
			private UI.RenderableProxy m_renderableProxy;

			public ZoomBackground()
			{
				m_toScreenSpace = Matrix.Identity;
				m_toScreenSpace.M11 = 2 / 1024.0f;
				m_toScreenSpace.M22 = -2 / 768.0f;
				m_toScreenSpace.M41 = -1.0f;
				m_toScreenSpace.M42 = 1.0f;

				m_renderableProxy = new UI.RenderableProxy(this);
			}

			public void OnRender(UI.RenderEventArgs e)
			{
				var gameui = GameApp.Service<GameUI>();

				// draw a black overlay
				Graphics.TexturedQuad quadOverlay = new Graphics.TexturedQuad();
				quadOverlay.ColorScale = new Vector4(0, 0, 0, gameui.m_zoomFadeTrack.CurrentValue * 0.75f);
				e.RenderManager.Draw(quadOverlay, new Point(-0.5f, -0.5f), m_toScreenSpace);

				if (gameui.m_zoomFadeTrack.CurrentValue == 0.0f)
				{
					Dispatcher = null;
				}
			}

			public void RaiseEvent(UI.MouseMoveEventArgs e)
			{
				e.SetHandled();
			}

			public void RaiseEvent(UI.MouseButton1EventArgs e)
			{
				e.SetHandled();
			}

			public void RaiseEvent(UI.MouseButton2EventArgs e)
			{
				var gameui = GameApp.Service<GameUI>();

				if (gameui.ZoomedInCard != null)
				{
					if (!e.State.Button2Pressed)
					{
						gameui.ZoomedInCard = null;
						gameui.m_zoomFadeTrack.TimeFactor = -1.0f;
						if (!gameui.m_zoomFadeTrack.IsPlaying)
						{
							gameui.m_zoomFadeTrack.PlayFrom(gameui.m_zoomFadeTrack.Time);
						}
					}
					e.SetHandled();
				}
			}
		}

		private ZoomBackground m_zoomBackground = new ZoomBackground();
		private Animation.Track m_zoomFadeTrack = new Animation.LinearTrack(0.35f);

		public UI.CardControl ZoomedInCard
		{
			get; private set;
		}

		public void ZoomInCard(UI.CardControl cardControl)
		{
			if (cardControl == null)
			{
				throw new ArgumentNullException("cardControl");
			}

            if (cardControl.GetAddin<UI.CardControlAddins.Flip>().Flipped)
            {
                // flipped card can't be viewed
                return;
            }

			if (ZoomedInCard != cardControl)
			{
				m_zoomFadeTrack.TimeFactor = 1.0f;
				if (!m_zoomFadeTrack.IsPlaying)
				{
					m_zoomFadeTrack.Play();
				}

				m_zoomBackground.Dispatcher = InGameUIPage.Style.ChildIds["ZoomedIn"].Target;
				ZoomedInCard = cardControl;
			}
		}

		private void UpdateZoomInCard(float deltaTime)
		{
			if (m_zoomFadeTrack.IsPlaying)
			{
				m_zoomFadeTrack.Elapse(deltaTime);
			}
		}
	}
}
