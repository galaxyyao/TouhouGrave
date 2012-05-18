using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaRect = Microsoft.Xna.Framework.Rectangle;
using XnaEffect = Microsoft.Xna.Framework.Graphics.Effect;

namespace TouhouSpring.Particle
{
	partial class Canvas
	{
		private struct ParticleVertex : IVertexType
		{
			public Vector3 position;
			public Byte corner;
			public Byte alignment;
#pragma warning disable 649 // Field '' is never assigned to, and will always have its default value 0
			public UInt16 padding;
#pragma warning restore 649
			public Vector4 uvparams;
			public Vector2 size;
			public float rotation;
			public Color color;

			private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.Position, 1),
				new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
				new VertexElement(44, VertexElementFormat.Color, VertexElementUsage.Color, 0)
			);

			VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
		};

		private XnaEffect m_effect;
		private EffectParameter m_paramCorners;
		private EffectParameter m_paramAlignments;
		private EffectParameter m_paramTransform;
		private EffectParameter m_paramTexture;
		private Texture2D m_whiteTexture;

		private int m_bufferCapacity = 0;
		private DynamicVertexBuffer m_vertices;
		private DynamicIndexBuffer m_indices;

		private ParticleVertex[] m_shadowedVertices;
		private UInt16[] m_shadowedIndices;

		private BlendState m_additiveBlend;
		private BlendState m_alphaBlend;
		private BlendState m_multiplyBlend;
		private DepthStencilState m_depthDisabled;

		private void Initialize_Render()
		{
			m_effect = new XnaEffect(GraphicsDevice, EffectLoader.CompileFromFile(
				Program.PathUtils.ToDiskPath("Effects/ParticleRenderer", "fx")));
			m_effect.CurrentTechnique = m_effect.Techniques[0];

			m_paramCorners = m_effect.Parameters["Corners"];
			m_paramAlignments = m_effect.Parameters["Alignments"];
			m_paramTransform = m_effect.Parameters["Transform"];
			m_paramTexture = m_effect.Parameters["TheTexture"];

			string whiteTexturePath = Program.PathUtils.ToDiskPath("Textures/White", "png");
			using (var whiteTextureStream = new FileStream(whiteTexturePath, FileMode.Open))
			{
				m_whiteTexture = Texture2D.FromStream(GraphicsDevice, whiteTextureStream);
			}

			m_additiveBlend = new BlendState
			{
				ColorSourceBlend = Blend.SourceAlpha,
				AlphaSourceBlend = Blend.SourceAlpha,
				ColorDestinationBlend = Blend.One,
				AlphaDestinationBlend = Blend.One
			};

			m_alphaBlend = new BlendState
			{
				ColorSourceBlend = Blend.SourceAlpha,
				AlphaSourceBlend = Blend.SourceAlpha,
				ColorDestinationBlend = Blend.InverseSourceAlpha,
				AlphaDestinationBlend = Blend.InverseSourceAlpha
			};

			m_multiplyBlend = new BlendState
			{
				ColorSourceBlend = Blend.Zero,
				AlphaSourceBlend = Blend.Zero,
				ColorDestinationBlend = Blend.SourceColor,
				AlphaDestinationBlend = Blend.SourceColor
			};

			m_depthDisabled = new DepthStencilState
			{
				DepthBufferEnable = false
			};
		}

		private void CreateGeometry(int capacity)
		{
			if (m_vertices != null)
			{
				m_vertices.Dispose();
				m_indices.Dispose();
			}

			m_vertices = new DynamicVertexBuffer(GraphicsDevice, typeof(ParticleVertex), capacity * 4, BufferUsage.WriteOnly);
			m_indices = new DynamicIndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, capacity * 6, BufferUsage.WriteOnly);

			m_shadowedVertices = new ParticleVertex[capacity * 4];
			m_shadowedIndices = new UInt16[capacity * 6];

			for (int i = 0; i < m_shadowedVertices.Length; ++i)
			{
				m_shadowedVertices[i].corner = (Byte)(i & 0x3);
			}
		}

		public void Render()
		{
			var numParticles = System.TotalLiveParticles;
			if (numParticles > m_bufferCapacity)
			{
				CreateGeometry(numParticles);
				m_bufferCapacity = numParticles;
			}

			if (m_bufferCapacity == 0 || numParticles == 0)
			{
				return;
			}

			UpdateVertices();
			GraphicsDevice.SetVertexBuffer(m_vertices);

			UpdateIndices();
			GraphicsDevice.Indices = m_indices;

			UpdateParameters();

			var oldBlend = GraphicsDevice.BlendState;
			switch (System.BlendMode)
			{
				default:
				case BlendMode.None:
					GraphicsDevice.BlendState = BlendState.Opaque;
					break;

				case BlendMode.Alpha:
					GraphicsDevice.BlendState = m_alphaBlend;
					break;

				case BlendMode.Additive:
					GraphicsDevice.BlendState = m_additiveBlend;
					break;

				case BlendMode.Multiply:
					GraphicsDevice.BlendState = m_multiplyBlend;
					break;
			}

			var oldDepth = GraphicsDevice.DepthStencilState;
			GraphicsDevice.DepthStencilState = m_depthDisabled;

			foreach (var pass in m_effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numParticles * 4, 0, numParticles * 2);
			}

			GraphicsDevice.BlendState = oldBlend;
			GraphicsDevice.DepthStencilState = oldDepth;
		}

		private void UpdateVertices()
		{
			var texture = System.TextureObject ?? m_whiteTexture;
			float invTexWidth = 1f / texture.Width;
			float invTexHeight = 1f / texture.Height;

			int writePos = 0;
			foreach (var effect in System.Effects)
			{
				XnaRect uvBounds = effect.UVBounds;

				var uvParams = new Vector4();
				uvParams.X = (float)uvBounds.Width * invTexWidth;
				uvParams.Y = -(float)uvBounds.Height * invTexHeight;
				uvParams.Z = 0.5f * uvParams.X + (uvBounds.X - 0.5f) * invTexWidth;
				uvParams.W = -0.5f * uvParams.Y + (uvBounds.Y - 0.5f) * invTexHeight;

				Byte alignment;
				switch (effect.Alignment)
				{
					default:
					case Alignment.Screen:
						alignment = 0;
						break;
					case Alignment.WorldXY:
						alignment = 1;
						break;
					case Alignment.WorldXZ:
						alignment = 2;
						break;
					case Alignment.WorldYZ:
						alignment = 3;
						break;
				}

				effect.BatchProcess((particles, begin, end) =>
				{
					for (int i = begin; i < end; ++i)
					{
						var p = particles[i];

						m_shadowedVertices[writePos].position = p.m_position;
						m_shadowedVertices[writePos].alignment = alignment;
						m_shadowedVertices[writePos].uvparams = uvParams;
						m_shadowedVertices[writePos].size = p.m_size;
						m_shadowedVertices[writePos].rotation = p.m_rotation;
						m_shadowedVertices[writePos].color = p.m_color;
						++writePos;

						m_shadowedVertices[writePos].position = p.m_position;
						m_shadowedVertices[writePos].alignment = alignment;
						m_shadowedVertices[writePos].uvparams = uvParams;
						m_shadowedVertices[writePos].size = p.m_size;
						m_shadowedVertices[writePos].rotation = p.m_rotation;
						m_shadowedVertices[writePos].color = p.m_color;
						++writePos;

						m_shadowedVertices[writePos].position = p.m_position;
						m_shadowedVertices[writePos].alignment = alignment;
						m_shadowedVertices[writePos].uvparams = uvParams;
						m_shadowedVertices[writePos].size = p.m_size;
						m_shadowedVertices[writePos].rotation = p.m_rotation;
						m_shadowedVertices[writePos].color = p.m_color;
						++writePos;

						m_shadowedVertices[writePos].position = p.m_position;
						m_shadowedVertices[writePos].alignment = alignment;
						m_shadowedVertices[writePos].uvparams = uvParams;
						m_shadowedVertices[writePos].size = p.m_size;
						m_shadowedVertices[writePos].rotation = p.m_rotation;
						m_shadowedVertices[writePos].color = p.m_color;
						++writePos;
					}
				});
			}
			m_vertices.SetData(m_shadowedVertices, 0, writePos, SetDataOptions.Discard);
		}

		private void UpdateIndices()
		{
			var len = System.TotalLiveParticles * 6;
			for (int i = 0; i < len; )
			{
				int i2 = m_sorter.SortedIndices[i / 6] * 4;
				m_shadowedIndices[i++] = (UInt16)(i2 + 0);
				m_shadowedIndices[i++] = (UInt16)(i2 + 1);
				m_shadowedIndices[i++] = (UInt16)(i2 + 2);
				m_shadowedIndices[i++] = (UInt16)(i2 + 2);
				m_shadowedIndices[i++] = (UInt16)(i2 + 1);
				m_shadowedIndices[i++] = (UInt16)(i2 + 3);
			}
			m_indices.SetData(m_shadowedIndices, 0, len, SetDataOptions.Discard);
		}

		private void UpdateParameters()
		{
			m_paramCorners.SetValue(new Vector2[] {
				new Vector2(-0.5f,  0.5f),
				new Vector2( 0.5f,  0.5f),
				new Vector2(-0.5f, -0.5f),
				new Vector2( 0.5f, -0.5f),
			});

			var viewProj = ViewMatrix * ProjectionMatrix;
			var invViewProj = Matrix.Invert(ViewMatrix * ProjectionMatrix);

			var alignScreen = Matrix.CreateScale(ProjectionMatrix.M22 / GraphicsDevice.Viewport.AspectRatio,
												 ProjectionMatrix.M22,
												 1) * invViewProj;
			m_paramAlignments.SetValue(new Matrix[] {
				alignScreen,
				new Matrix { M11 = 1, M22 = 1 },
				new Matrix { M11 = 1, M23 = 1 },
				new Matrix { M12 = 1, M23 = 1 }
			});

			m_paramTransform.SetValue(viewProj);
			m_paramTexture.SetValue(System.TextureObject ?? m_whiteTexture);
		}
	}
}
