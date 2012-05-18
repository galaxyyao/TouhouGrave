using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaCurve = Microsoft.Xna.Framework.Curve;

namespace TouhouSpring.Services
{
	partial class MenuUI
	{
		class MenuPage
		{
			private Animation.Track m_enterAnimation;
			private Animation.Track m_leaveAnimation;

			public UI.Page Page
			{
				get; private set;
			}

			public List<UI.MenuItem> Items
			{
				get; private set;
			}

			public delegate void MenuEventHandler(string id, UI.MenuItem item);
			public event MenuEventHandler MenuClicked;

			public MenuPage(UI.Page page)
			{
				if (page == null)
				{
					throw new ArgumentNullException("page");
				}

				Page = page;
				Items = new List<UI.MenuItem>();

				foreach (var idChild in Page.Style.ChildIds)
				{
					var menuItem = idChild.Value.Target as UI.MenuItem;
					if (menuItem != null)
					{
						Items.Add(menuItem);
						string menuId = idChild.Key;
						menuItem.MouseButton1Up += (s, e) => OnMenuItemClicked(menuItem, menuId);
					}
				}

				var curve = GameApp.Service<ResourceManager>().Acquire<XnaCurve>("Curve_CardMove");

				m_enterAnimation = new Animation.CurveTrack(curve);
				m_enterAnimation.Elapsed += weight =>
				{
					foreach (var menuItem in Items)
					{
						menuItem.Transform *= MatrixHelper.Translate(0, 50 * (1 - weight));
                        var clr = menuItem.Label.TextColor;
                        clr.A = (byte)(weight * 255);
						menuItem.Label.TextColor = clr;
					}
				};

				m_leaveAnimation = new Animation.CurveTrack(curve);
				m_leaveAnimation.Elapsed += weight =>
				{
					foreach (var menuItem in Items)
					{
						menuItem.Transform *= MatrixHelper.Translate(0, 50 * weight);
                        var clr = menuItem.Label.TextColor;
                        clr.A = (byte)(255 - weight * 255);
						menuItem.Label.TextColor = clr;
					}

					if (!m_leaveAnimation.IsPlaying)
					{
						Page.Dispatcher = null;
					}
				};
			}

			public void PlayEnterAnimation()
			{
				if (m_leaveAnimation.IsPlaying)
				{
					m_leaveAnimation.Stop();
				}
				if (m_enterAnimation.IsPlaying)
				{
					m_enterAnimation.Stop();
				}
				m_enterAnimation.Play();
			}

			public void PlayLeaveAnimation()
			{
				if (m_enterAnimation.IsPlaying)
				{
					m_enterAnimation.Stop();
				} 
				if (m_leaveAnimation.IsPlaying)
				{
					m_leaveAnimation.Stop();
				}
				m_leaveAnimation.Play();
			}

			public void Update(float deltaTime)
			{
				if (m_enterAnimation.IsPlaying)
				{
					m_enterAnimation.Elapse(deltaTime);
				}
				else if (m_leaveAnimation.IsPlaying)
				{
					m_leaveAnimation.Elapse(deltaTime);
				}
			}

			private void OnMenuItemClicked(UI.MenuItem sender, string id)
			{
				if (MenuClicked != null)
				{
					MenuClicked(id, sender);
				}
			}
		}

		private MenuPage m_currentPage = null;

		private MenuPage CurrentPage
		{
			get { return m_currentPage; }
			set
			{
				if (value != null && !m_pages.ContainsValue(value))
				{
					throw new ArgumentException("Unrecognized page.");
				}

				if (m_currentPage != null)
				{
					m_currentPage.PlayLeaveAnimation();
				}

				m_currentPage = value;

				if (m_currentPage != null)
				{
					m_currentPage.Page.Dispatcher = Root;
					m_currentPage.PlayEnterAnimation();
				}
			}
		}
	}
}
