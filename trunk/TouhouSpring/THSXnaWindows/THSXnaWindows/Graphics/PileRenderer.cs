using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
    [Services.LifetimeDependency(typeof(Services.ResourceManager))]
    class PileRenderer : Services.GameService
    {
        struct Vertex : IVertexType
        {
            public Vector3 pos;
            public Vector2 uv;

            private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

            VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
        }

        private Effect m_effect;
        private EffectParameter m_paramTransform;
        private EffectParameter m_paramPileSize;
        private EffectParameter m_paramUVAdjust;
        private EffectParameter m_paramColorBias;
        private EffectParameter m_paramColorScale;
        private EffectParameter m_paramTexture;

        private VertexBuffer m_pileVertices;
        private IndexBuffer m_pileIndices;

        public void Draw(VirtualTexture cardFrameTexture, Vector3 pileSize, Matrix transform)
        {
            if (cardFrameTexture == null)
            {
                throw new ArgumentNullException("cardFrameTexture");
            }

            PInvokes.D3d9.BeginPixEvent(0, "PileRenderer.Draw");

            var device = GameApp.Instance.GraphicsDevice;

            var oldBlendState = device.BlendState;
            var oldDepthState = device.DepthStencilState;
            var oldRasterizerState = device.RasterizerState;

            device.SetVertexBuffer(m_pileVertices);
            device.Indices = m_pileIndices;
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.RasterizerState = RasterizerState.CullCounterClockwise;

            m_paramTransform.SetValue(transform);
            m_paramPileSize.SetValue(pileSize);
            var oneOverWidth = 1.0f / cardFrameTexture.XnaTexture.Bounds.Width;
            var oneOverHeight = 1.0f / cardFrameTexture.XnaTexture.Bounds.Height;
            m_paramUVAdjust.SetValue(new Vector4(
                cardFrameTexture.Bounds.Width * oneOverWidth,
                cardFrameTexture.Bounds.Height * oneOverHeight,
                cardFrameTexture.Bounds.Left * oneOverWidth,
                cardFrameTexture.Bounds.Top * oneOverHeight));
            m_paramColorBias.SetValue(Vector4.Zero);
            m_paramColorScale.SetValue(Vector4.One);
            m_paramTexture.SetValue(cardFrameTexture.XnaTexture);

            foreach (var pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 10);
            }

            device.BlendState = oldBlendState;
            device.DepthStencilState = oldDepthState;
            device.RasterizerState = oldRasterizerState;

            PInvokes.D3d9.EndPixEvent();
        }

        public override void Startup()
        {
            m_effect = GameApp.Service<Services.ResourceManager>().Acquire<Effect>("Effects/PileRenderer");
            m_effect.CurrentTechnique = m_effect.Techniques[0];
            m_paramTransform = m_effect.Parameters["Transform"];
            m_paramPileSize = m_effect.Parameters["PileSize"];
            m_paramUVAdjust = m_effect.Parameters["UVAdjust"];
            m_paramColorBias = m_effect.Parameters["ColorBias"];
            m_paramColorScale = m_effect.Parameters["ColorScale"];
            m_paramTexture = m_effect.Parameters["TheTexture"];

            var vertices = new Vertex[]
            {
                new Vertex { pos = new Vector3(0, 0, 0), uv = new Vector2(0, 0) },
                new Vertex { pos = new Vector3(1, 0, 0), uv = new Vector2(1, 0) },
                new Vertex { pos = new Vector3(0, 1, 0), uv = new Vector2(0, 1) },
                new Vertex { pos = new Vector3(1, 1, 0), uv = new Vector2(1, 1) },
                new Vertex { pos = new Vector3(0, 0, 1), uv = new Vector2(0, 0) },
                new Vertex { pos = new Vector3(1, 0, 1), uv = new Vector2(1, 0) },
                new Vertex { pos = new Vector3(0, 1, 1), uv = new Vector2(0, 1) },
                new Vertex { pos = new Vector3(1, 1, 1), uv = new Vector2(1, 1) }
            };
            m_pileVertices = new VertexBuffer(GameApp.Instance.GraphicsDevice, typeof(Vertex), vertices.Length, BufferUsage.None);
            m_pileVertices.SetData(vertices);

            var indices = new UInt16[]
            {
                1, 0, 3, 3, 0, 2, // bottom
                4, 6, 0, 0, 6, 2, // left
                7, 5, 3, 3, 5, 1, // right
                6, 7, 2, 2, 7, 3, // front
                5, 4, 1, 1, 4, 0  // back
            };
            m_pileIndices = new IndexBuffer(GameApp.Instance.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            m_pileIndices.SetData(indices);
        }

        public override void Shutdown()
        {
            m_pileIndices.Dispose();
            m_pileVertices.Dispose();
            m_effect.Dispose();
        }
    }
}
