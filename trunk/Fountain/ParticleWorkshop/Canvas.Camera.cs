using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle
{
	partial class Canvas
	{
		private Vector3 m_cameraTarget;
		private float m_cameraDistance;
		private float m_cameraRotationTheta;
		private float m_cameraRotationPhi;
		private float m_fov;
		private float m_nearClip;
		private float m_farClip;

		private bool m_leftMouseDown = false;
		private bool m_rightMouseDown = false;
		private Vector3 m_mouseDownCameraTarget;
		private float m_mouseDownRotationTheta;
		private float m_mouseDownRotationPhi;
		private System.Drawing.Point m_mouseDownLocation;

		public Matrix ViewMatrix
		{
			get; private set;
		}

		public Matrix ProjectionMatrix
		{
			get; private set;
		}

		public void Initialize_Camera()
		{
			m_cameraTarget = Vector3.Zero;
			m_cameraDistance = 100.0f;
			m_cameraRotationTheta = 45.0f;
			m_cameraRotationPhi = 30.0f;

			m_fov = 45.0f;
			m_nearClip = 1.0f;
			m_farClip = 1000.0f;

			UpdateViewMatrix();
			UpdateProjectionMatrix();
		}

		private void UpdateViewMatrix()
		{
			Vector3 camPos = -Vector3.UnitY;
			Vector3 camUp = Vector3.UnitZ;

			Matrix camMatrix = Matrix.CreateRotationX(-MathHelper.ToRadians(m_cameraRotationPhi))
							   * Matrix.CreateRotationZ(-MathHelper.ToRadians(m_cameraRotationTheta));

			camPos = Vector3.Transform(camPos, camMatrix) * m_cameraDistance;
			camUp = Vector3.TransformNormal(camUp, camMatrix);

			ViewMatrix = Matrix.CreateLookAt(camPos + m_cameraTarget, m_cameraTarget, camUp);
		}

		private void UpdateProjectionMatrix()
		{
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(m_fov),
				GraphicsDevice.Viewport.AspectRatio, m_nearClip, m_farClip);
		}

		private void OnMouseDown_Camera(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				m_mouseDownCameraTarget = m_cameraTarget;
				m_mouseDownLocation = e.Location;
				m_leftMouseDown = true;
			}
			else if (e.Button == MouseButtons.Right)
			{
				m_mouseDownRotationTheta = m_cameraRotationTheta;
				m_mouseDownRotationPhi = m_cameraRotationPhi;
				m_mouseDownLocation = e.Location;
				m_rightMouseDown = true;
			}
            Focus();
		}

		private void OnMouseUp_Camera(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				m_leftMouseDown = false;
			}
			else if (e.Button == MouseButtons.Right)
			{
				m_rightMouseDown = false;
			}
		}

		private void OnMouseMove_Camera(object sender, MouseEventArgs e)
		{
			int dx = e.Location.X - m_mouseDownLocation.X;
			int dy = e.Location.Y - m_mouseDownLocation.Y;

			if (m_leftMouseDown)
			{
				dx /= 4;
				dy /= 4;

				var left = new Vector3(-(float)Math.Sin(MathHelper.ToRadians(m_cameraRotationTheta + 90)),
									   -(float)Math.Cos(MathHelper.ToRadians(m_cameraRotationTheta + 90)),
									   0);
				var forward = new Vector3(-(float)Math.Sin(MathHelper.ToRadians(m_cameraRotationTheta)),
										  -(float)Math.Cos(MathHelper.ToRadians(m_cameraRotationTheta)),
										  0);
				m_cameraTarget = m_mouseDownCameraTarget + left * dx - forward * dy;
			}
			else if (m_rightMouseDown)
			{
				m_cameraRotationTheta = m_mouseDownRotationTheta + dx;
				m_cameraRotationPhi = m_mouseDownRotationPhi + dy;
			}
		}

		private void OnMouseWheel_Camera(object sender, MouseEventArgs e)
		{
			if (ClientRectangle.Contains(e.Location))
			{
				int delta = e.Delta / 120;
				m_cameraDistance *= (float)Math.Pow(0.8, delta);
			}
		}
	}
}
