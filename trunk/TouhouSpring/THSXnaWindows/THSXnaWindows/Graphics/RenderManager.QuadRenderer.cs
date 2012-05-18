using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
	[Services.LifetimeDependency(typeof(Services.ResourceManager))]
	partial class RenderManager
	{
		private Effect m_effect;
		private EffectTechnique m_techNormal;
		private EffectTechnique m_techTone;
		private EffectTechnique m_techSimple;
		private EffectParameter m_paramTransform;
		private EffectParameter m_paramPosAdjust;
		private EffectParameter m_paramUVAdjust;
		private EffectParameter m_paramColorBias;
		private EffectParameter m_paramColorScale;
        private EffectParameter m_paramToneParameters;
		private EffectParameter m_paramTexture;
		private Texture2D m_whiteTexture;
		private VertexBuffer m_vertexBuffer;
		private VertexDeclaration m_vertexDeclaration;
		private DepthStencilState[] m_depthSettings;

		private Stack<EffectTechnique> m_techStack = new Stack<EffectTechnique>();

		public DepthStencilState OverridingDepthStencilState
		{
			get; set;
		}

		public void PushNormalTechnique()
		{
			m_techStack.Push(m_techNormal);
		}

		public void PushToneTechnique(float saturate, float brightness)
		{
            m_paramToneParameters.SetValue(new Vector2(saturate, brightness));
			m_techStack.Push(m_techTone);
		}

		public void PushSimpleTechnique()
		{
			m_techStack.Push(m_techSimple);
		}

		public void PopTechnique()
		{
			m_techStack.Pop();
		}

		public void Draw(TexturedQuad quad, Point position, Matrix transform)
		{
			float width = quad.Texture != null ? quad.Texture.Width : Device.Viewport.Width;
			float height = quad.Texture != null ? quad.Texture.Height : Device.Viewport.Height;
			Draw(quad, new Rectangle(position, new Size(width, height)), transform);
		}

		public void Draw(TexturedQuad quad, Rectangle rect, Matrix transform)
		{
			if (quad == null)
			{
				throw new ArgumentNullException("quad");
			}

			Device.SetVertexBuffer(m_vertexBuffer);
			Device.Indices = null;
			Device.BlendState = quad.BlendState;
			Device.DepthStencilState = OverridingDepthStencilState ?? m_depthSettings[(quad.ZTest ? 2 : 0) | (quad.ZWrite ? 1 : 0)];

			if (m_effect.CurrentTechnique != m_techStack.Peek())
			{
				m_effect.CurrentTechnique = m_techStack.Peek();
			}

			m_paramTransform.SetValue(transform);
			m_paramPosAdjust.SetValue(new Vector4(rect.Width, rect.Height, rect.Left, rect.Top));

			float uvWidth = quad.Texture != null ? quad.Texture.XnaTexture.Width : Device.Viewport.Width;
			float uvHeight = quad.Texture != null ? quad.Texture.XnaTexture.Height : Device.Viewport.Height;
			float uvLeft = quad.UVBounds.Left
						   + (quad.Texture != null ? quad.Texture.Bounds.Left : 0)
						   + (quad.OffsetByHalfTexel ? 0.5f : 0.0f);
			float uvTop = quad.UVBounds.Top
						  + (quad.Texture != null ? quad.Texture.Bounds.Top : 0)
						  + (quad.OffsetByHalfTexel ? 0.5f : 0.0f);
			m_paramUVAdjust.SetValue(new Vector4(quad.UVBounds.Width / uvWidth, quad.UVBounds.Height / uvHeight,
												 uvLeft / uvWidth, uvTop / uvHeight));

			m_paramColorBias.SetValue(quad.ColorToAdd.ToVector4());
			m_paramColorScale.SetValue(quad.ColorToModulate.ToVector4() * quad.ColorScale);
			m_paramTexture.SetValue(m_techStack.Peek() != m_techSimple
									? quad.Texture != null ? quad.Texture.XnaTexture : m_whiteTexture
									: null);

			foreach (var pass in m_effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			}
		}

		private void CreateQuadRenderer()
		{
			CreateQuadGeometry();
			CreateQuadMaterial();
			m_whiteTexture = GameApp.Service<Services.ResourceManager>().Acquire<Texture2D>("Textures/White");

			m_depthSettings = new DepthStencilState[4];
			for (int i = 0; i < 4; ++i)
			{
				bool zTest = (i & 2) != 0;
				bool zWrite = (i & 1) != 0;
				m_depthSettings[i] = new DepthStencilState();
				m_depthSettings[i].DepthBufferEnable = zTest || zWrite;
				if (zTest || zWrite)
				{
					m_depthSettings[i].DepthBufferFunction = zTest ? CompareFunction.LessEqual : CompareFunction.Always;
					m_depthSettings[i].DepthBufferWriteEnable = zWrite;
				}
			}
		}

		private void DestroyQuadRenderer()
		{
			GameApp.Service<Services.ResourceManager>().Release(m_whiteTexture);
			GameApp.Service<Services.ResourceManager>().Release(m_effect);
			m_vertexBuffer.Dispose();
			m_vertexDeclaration.Dispose();
		}

		private void CreateQuadGeometry()
		{
			m_vertexDeclaration = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
			);
			m_vertexBuffer = new VertexBuffer(Device, m_vertexDeclaration, 4, BufferUsage.None);

			Vector2[] vertices = {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(0, 1),
				new Vector2(1, 1),
			};
			m_vertexBuffer.SetData(vertices);
		}

		private void CreateQuadMaterial()
		{
			m_effect = GameApp.Service<Services.ResourceManager>().Acquire<Effect>("Effects/QuadRenderer");
			m_techNormal = m_effect.Techniques["Normal"];
			m_techTone = m_effect.Techniques["Tone"];
			m_techSimple = m_effect.Techniques["Simple"];

			m_techStack.Push(m_techNormal);

			// parameter mappings
			m_paramTransform = m_effect.Parameters["Transform"];
			m_paramPosAdjust = m_effect.Parameters["PosAdjust"];
			m_paramUVAdjust = m_effect.Parameters["UVAdjust"];
			m_paramColorBias = m_effect.Parameters["ColorBias"];
			m_paramColorScale = m_effect.Parameters["ColorScale"];
            m_paramToneParameters = m_effect.Parameters["ToneParameters"];
			m_paramTexture = m_effect.Parameters["TheTexture"];
		}
	}
}
