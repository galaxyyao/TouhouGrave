using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Graphics
{
	[Services.LifetimeDependency(typeof(Services.ResourceManager))]
	partial class ParticleRenderer : Services.GameService
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
			public Particle.Color color;

			private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.Position, 1),
				new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
				new VertexElement(44, VertexElementFormat.Color, VertexElementUsage.Color, 0)
			);
			public static int Size { get { return s_vertDecl.VertexStride; } }

			VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
		};

		private ParticleVertex[] m_shadowedVertices;
		private UInt16[] m_shadowedIndices;
		private Particle.ParticleSorter m_sorter = new Particle.ParticleSorter();

		private const int BufferCapacity = 500;
		private int m_bufferWriteCursor = 0;
		private DynamicVertexBuffer m_vertices;
		private DynamicIndexBuffer m_indices;

		private Effect m_effect;
		private EffectParameter m_paramCorners;
		private EffectParameter m_paramAlignments;
		private EffectParameter m_paramTransform;
		private EffectParameter m_paramTexture;
		private Texture2D m_whiteTexture;

		private BlendState m_multiplyBlend;
		private DepthStencilState m_depthDisabled;

		public void Draw(Particle.ParticleSystem system, Matrix transform, float screenScale, float aspectRatio)
		{
			if (system == null)
			{
				throw new ArgumentNullException("system");
			}

			var numParticles = system.TotalLiveParticles;
			if (numParticles == 0)
			{
				return;
			}

			m_sorter.Sort(system, Vector3.UnitZ);

			FillShadowedVertices(system);
			FillShadowedIndices();

			UpdateParameters(system, transform, screenScale, aspectRatio);

			var device = GameApp.Instance.GraphicsDevice;

			var oldBlend = device.BlendState;
			switch (system.BlendMode)
			{
				default:
				case Particle.BlendMode.None:
					device.BlendState = BlendState.Opaque;
					break;

				case Particle.BlendMode.Alpha:
					device.BlendState = BlendState.AlphaBlend;
					break;

				case Particle.BlendMode.Additive:
					device.BlendState = BlendState.Additive;
					break;

				case Particle.BlendMode.Multiply:
					device.BlendState = m_multiplyBlend;
					break;
			}

			device.SetVertexBuffer(m_vertices);
			device.Indices = m_indices;

			var oldDepth = device.DepthStencilState;
			device.DepthStencilState = m_depthDisabled;

			SetDataOptions hint = SetDataOptions.NoOverwrite;
			if (m_bufferWriteCursor + numParticles > BufferCapacity)
			{
				hint = SetDataOptions.Discard;
				m_bufferWriteCursor = 0;
			}

			var vertSize = ParticleVertex.Size;
			m_vertices.SetData(m_bufferWriteCursor * 4 * vertSize, m_shadowedVertices, 0, numParticles * 4, vertSize, hint);
			m_indices.SetData(m_bufferWriteCursor * 6 * 2, m_shadowedIndices, 0, numParticles * 6, hint);

			foreach (var pass in m_effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, m_bufferWriteCursor * 4, 0, numParticles * 4, m_bufferWriteCursor * 6, numParticles * 2);
			}

			m_bufferWriteCursor += numParticles;

			device.BlendState = oldBlend;
			device.DepthStencilState = oldDepth;
		}

		public override void Startup()
		{
			Particle.ResourceLoader.Instance = new ResourceLoader();

			CreateGeometry(BufferCapacity);

			// located in ParticleContent
			m_effect = GameApp.Service<Services.ResourceManager>().Acquire<Effect>("Effects/ParticleRenderer");
			m_effect.CurrentTechnique = m_effect.Techniques[0];

			m_paramCorners = m_effect.Parameters["Corners"];
			m_paramAlignments = m_effect.Parameters["Alignments"];
			m_paramTransform = m_effect.Parameters["Transform"];
			m_paramTexture = m_effect.Parameters["TheTexture"];

			m_whiteTexture = GameApp.Service<Services.ResourceManager>().Acquire<Texture2D>("Textures/White");

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

		public override void Shutdown()
		{
			m_depthDisabled.Dispose();
			m_multiplyBlend.Dispose();
			GameApp.Service<Services.ResourceManager>().Release(m_whiteTexture);
			GameApp.Service<Services.ResourceManager>().Release(m_effect);

			m_indices.Dispose();
			m_vertices.Dispose();
		}

		private void CreateGeometry(int capacity)
		{
			var device = GameApp.Instance.GraphicsDevice;
			m_vertices = new DynamicVertexBuffer(device, typeof(ParticleVertex), capacity * 4, 0);
			m_indices = new DynamicIndexBuffer(device, IndexElementSize.SixteenBits, capacity * 6, 0);
		}

		private void FillShadowedVertices(Particle.ParticleSystem system)
		{
			if (m_shadowedVertices == null || m_shadowedVertices.Length < m_sorter.SortedIndices.Length * 4)
			{
				m_shadowedVertices = new ParticleVertex[m_sorter.SortedIndices.Length * 4];
				for (int i = 0; i < m_shadowedVertices.Length; ++i)
				{
					m_shadowedVertices[i].corner = (Byte)(i & 0x3);
				}
			}

			var texture = system.TextureObject ?? m_whiteTexture;
			float invTexWidth = 1f / texture.Width;
			float invTexHeight = 1f / texture.Height;

			int writePos = 0;
			foreach (var effect in system.Effects)
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
					case Particle.Alignment.Screen:
						alignment = 0;
						break;
					case Particle.Alignment.WorldXY:
						alignment = 1;
						break;
					case Particle.Alignment.WorldXZ:
						alignment = 2;
						break;
					case Particle.Alignment.WorldYZ:
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
		}

		private void FillShadowedIndices()
		{
			if (m_shadowedIndices == null || m_shadowedIndices.Length < m_sorter.SortedIndices.Length * 6)
			{
				m_shadowedIndices = new UInt16[m_sorter.SortedIndices.Length * 6];
			}

			var len = m_sorter.SortedIndices.Length * 6;
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
		}

		private void UpdateParameters(Particle.ParticleSystem system, Matrix transform, float aspectRatio, float screenScale)
		{
			m_paramCorners.SetValue(new Vector2[] {
				new Vector2(-0.5f,  0.5f),
				new Vector2( 0.5f,  0.5f),
				new Vector2(-0.5f, -0.5f),
				new Vector2( 0.5f, -0.5f),
			});

			var device = GameApp.Instance.GraphicsDevice;
			m_paramAlignments.SetValue(new Matrix[] {
				Matrix.CreateScale(screenScale / aspectRatio, screenScale, 1) * Matrix.Invert(transform),
				new Matrix { M11 = 1, M22 = 1 },
				new Matrix { M11 = 1, M23 = 1 },
				new Matrix { M12 = 1, M23 = 1 }
			});

			m_paramTransform.SetValue(transform);
			m_paramTexture.SetValue(system.TextureObject ?? m_whiteTexture);
		}
	}
}
