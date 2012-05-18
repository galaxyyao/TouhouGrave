using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
	class SelectedAnimation : CardControl.Addin, Style.IBindingProvider
	{
		private Animation.Track m_selectedTrack;
		private Matrix m_selectedTransform = MatrixHelper.Identity;
		private bool m_lastSelected = false;

		public SelectedAnimation(CardControl control) : base(control)
		{
			Control.Style.RegisterBinding(this);

			m_selectedTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardSelected"));
			m_selectedTrack.Elapsed += w =>
			{
				w = m_lastSelected ? w : 1 - w;
				m_selectedTransform = MatrixHelper.Translate(0, -w * 40.0f);
			};
		}

		public override void Update(float deltaTime)
		{
			var gameUI = GameApp.Service<Services.GameUI>();
			var selected = gameUI.ZoomedInCard != Control && gameUI.IsCardSelected(Card);

			if (selected != m_lastSelected)
			{
				if (m_selectedTrack.IsPlaying)
				{
					m_selectedTrack.Stop();
				}

				m_selectedTrack.Play();
				m_lastSelected = !m_lastSelected;
			}

			if (m_selectedTrack.IsPlaying)
			{
				m_selectedTrack.Elapse(deltaTime);
			}
		}

		public bool TryGetValue(string id, out string replacement)
		{
			switch (id)
			{
				case "CardAnimator.SelectedTransform":
					replacement = m_selectedTransform.Serialize();
					return true;

				default:
					replacement = null;
					return false;
			}
		}
	}
}
