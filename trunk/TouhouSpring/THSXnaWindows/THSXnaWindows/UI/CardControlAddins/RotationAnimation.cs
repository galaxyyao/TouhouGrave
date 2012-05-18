using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
	class RotationAnimation : CardControl.Addin, Style.IBindingProvider
	{
		private Animation.Track m_rotationTrack;
		private Matrix m_rotationTransform = MatrixHelper.Identity;
		private bool m_lastRotated = false;

		public RotationAnimation(CardControl control) : base(control)
		{
			Control.Style.RegisterBinding(this);

			m_rotationTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardSelected"));
			m_rotationTrack.Elapsed += w =>
			{
				w = m_lastRotated ? w : 1 - w;
				var halfWidth = Control.Region.Width * 0.5f;
				var halfHeight = Control.Region.Height * 0.5f;
				m_rotationTransform = MatrixHelper.Translate(-halfWidth, -halfHeight)
									* MatrixHelper.RotateZ(MathUtils.PI * 0.5f * w)
									* MatrixHelper.Translate(halfWidth, halfHeight);
			};
		}

		public override void Update(float deltaTime)
		{
			var needRotate = GameApp.Service<Services.GameUI>().ZoomedInCard != Control
							 && Card.State == CardState.CoolingDown;

			if (needRotate != m_lastRotated)
			{
				if (m_rotationTrack.IsPlaying)
				{
					m_rotationTrack.Stop();
				}

				m_rotationTrack.Play();
				m_lastRotated = !m_lastRotated;
			}

			if (m_rotationTrack.IsPlaying)
			{
				m_rotationTrack.Elapse(deltaTime);
			}
		}

		public bool TryGetValue(string id, out string replacement)
		{
			switch (id)
			{
				case "CardAnimator.RotationTransform":
					replacement = m_rotationTransform.Serialize();
					return true;

				default:
					replacement = null;
					return false;
			}
		}
	}
}
