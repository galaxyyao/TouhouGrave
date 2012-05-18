using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaVector = Microsoft.Xna.Framework.Vector3;

namespace TouhouSpring
{
	class Camera
	{
		private bool m_dirty = true;
		private XnaMatrix m_viewMatrix;
		private XnaMatrix m_projectionMatrix;
		private XnaMatrix m_toProjection;
		private XnaMatrix m_invToProjection;
		private XnaMatrix m_alignToNearPlane;

		public XnaMatrix PostWorldMatrix
		{
			get; set;
		}

		public XnaVector Position
		{
			get; set;
		}

		public XnaVector LookAt
		{
			get; set;
		}

		public XnaVector Up
		{
			get; set;
		}

		public bool IsPerspective
		{
			get; set;
		}

		public float NearPlane
		{
			get; set;
		}

		public float FarPlane
		{
			get; set;
		}

		public float ViewportWidth
		{
			get; set;
		}

		public float ViewportHeight
		{
			get; set;
		}

		public XnaMatrix ViewMatrix
		{
			get
			{
				UpdateMatrices();
				return m_viewMatrix;
			}
		}

		public XnaMatrix ProjectionMatrix
		{
			get
			{
				UpdateMatrices();
				return m_projectionMatrix;
			}
		}

		public XnaMatrix WorldToProjectionMatrix
		{
			get
			{
				UpdateMatrices();
				return m_toProjection;
			}
		}

		public XnaMatrix ProjectionToWorldMatrix
		{
			get
			{
				UpdateMatrices();
				return m_invToProjection;
			}
		}

		public XnaMatrix AlignToNearPlaneMatrix
		{
			get
			{
				UpdateMatrices();
				return m_alignToNearPlane;
			}
		}

		public Camera()
		{
			PostWorldMatrix = XnaMatrix.Identity;
			Position = XnaVector.UnitZ;
			LookAt = XnaVector.Zero;
			Up = XnaVector.UnitY;
			IsPerspective = false;
			NearPlane = 0.1f;
			FarPlane = 100.0f;
			ViewportWidth = 2.0f;
			ViewportHeight = 2.0f;
		}

		public void Dirty()
		{
			m_dirty = true;
		}

		private void UpdateMatrices()
		{
			if (m_dirty)
			{
				m_viewMatrix = XnaMatrix.CreateLookAt(Position, LookAt, Up);
				m_projectionMatrix = IsPerspective
									 ? XnaMatrix.CreatePerspective(ViewportWidth, ViewportHeight, NearPlane, FarPlane)
									 : XnaMatrix.CreateOrthographic(ViewportWidth, ViewportHeight, NearPlane, FarPlane);
				m_toProjection = PostWorldMatrix * m_viewMatrix * m_projectionMatrix;
				m_invToProjection = m_toProjection.Invert();

				var forward = XnaVector.Normalize(LookAt - Position);
				var position = Position + NearPlane * 1.01f * forward;
				var xAxis = XnaVector.Cross(forward, Up);
				var yAxis = XnaVector.Normalize(XnaVector.Cross(xAxis, forward));
				var mat = XnaMatrix.CreateWorld(position, forward, yAxis);
				var vpWidth = Math.Abs(ViewportWidth);
				var vpHeight = Math.Abs(ViewportHeight);
				m_alignToNearPlane = PostWorldMatrix * XnaMatrix.CreateScale(vpWidth * 0.5f, vpHeight * 0.5f, 1) * mat * PostWorldMatrix.Invert();

				m_dirty = false;
			}
		}
	}
}
