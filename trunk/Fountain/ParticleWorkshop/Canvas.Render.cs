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
        private struct ParticleVertex1 : IVertexType
        {
            public Vector3 position;
            public Byte corner;
            public Byte expand;
#pragma warning disable 649 // Field '' is never assigned to, and will always have its default value 0
            public UInt16 padding;
#pragma warning restore 649
            public Vector2 size;
            public float rotation;
            public Vector4 uvparams;
            public Color color;

            private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.Position, 1),
                new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Position, 2),
                new VertexElement(28, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(44, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );

            VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
        };

        private struct ParticleVertex2 : IVertexType
        {
            public Vector4 col0;
            public Vector4 col1;
            public Vector4 col2;
            public Vector4 col3;

            private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4)
            );

            VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
        };

        private XnaEffect m_effect;
        private EffectParameter m_paramCorners;
        private EffectParameter m_paramExpandMatrices;
        private EffectParameter m_paramTransform;
        private EffectParameter m_paramTexture;
        private Texture2D m_whiteTexture;

        private int m_bufferCapacity = 0;
        private DynamicVertexBuffer m_vertices1;
        private DynamicVertexBuffer m_vertices2;
        private DynamicIndexBuffer m_indices;

        private ParticleVertex1[] m_shadowedVertices1;
        private ParticleVertex2[] m_shadowedVertices2;
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
            m_paramExpandMatrices = m_effect.Parameters["ExpandMatrices"];
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
            if (m_vertices1 != null)
            {
                m_vertices1.Dispose();
                m_vertices2.Dispose();
                m_indices.Dispose();
            }

            m_vertices1 = new DynamicVertexBuffer(GraphicsDevice, typeof(ParticleVertex1), capacity * 4, BufferUsage.WriteOnly);
            m_vertices2 = new DynamicVertexBuffer(GraphicsDevice, typeof(ParticleVertex2), capacity * 4, BufferUsage.WriteOnly);
            m_indices = new DynamicIndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, capacity * 6, BufferUsage.WriteOnly);

            m_shadowedVertices1 = new ParticleVertex1[capacity * 4];
            m_shadowedVertices2 = new ParticleVertex2[capacity * 4];
            m_shadowedIndices = new UInt16[capacity * 6];

            for (int i = 0; i < m_shadowedVertices1.Length; ++i)
            {
                m_shadowedVertices1[i].corner = (Byte)(i & 0x3);
            }
        }

        public void Render()
        {
            var numParticles = SystemInstance.TotalLiveParticles;
            if (numParticles > m_bufferCapacity)
            {
                CreateGeometry(numParticles);
                m_bufferCapacity = numParticles;
            }

            if (m_bufferCapacity == 0 || numParticles == 0)
            {
                return;
            }

            UpdateParameters();

            UpdateVertices();
            GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(m_vertices1), new VertexBufferBinding(m_vertices2));

            UpdateIndices();
            GraphicsDevice.Indices = m_indices;

            var oldBlend = GraphicsDevice.BlendState;
            switch (SystemInstance.System.BlendMode)
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

            var oldRasterizer = GraphicsDevice.RasterizerState;
            GraphicsDevice.RasterizerState = SystemInstance.System.DoubleFaced ? RasterizerState.CullNone : RasterizerState.CullCounterClockwise;

            foreach (var pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numParticles * 4, 0, numParticles * 2);
            }

            GraphicsDevice.BlendState = oldBlend;
            GraphicsDevice.DepthStencilState = oldDepth;
            GraphicsDevice.RasterizerState = oldRasterizer;
        }

        private void UpdateVertices()
        {
            var texture = SystemInstance.System.TextureObject ?? m_whiteTexture;
            float invTexWidth = 1f / texture.Width;
            float invTexHeight = 1f / texture.Height;

            int writePos = 0;
            foreach (var effect in SystemInstance.EffectInstances)
            {
                XnaRect uvBounds = effect.Effect.UVBounds;

                var uvParams = new Vector4();
                uvParams.X = (float)uvBounds.Width * invTexWidth;
                uvParams.Y = -(float)uvBounds.Height * invTexHeight;
                uvParams.Z = 0.5f * uvParams.X + (uvBounds.X - 0.5f) * invTexWidth;
                uvParams.W = -0.5f * uvParams.Y + (uvBounds.Y - 0.5f) * invTexHeight;

                Byte expand = (Byte)(effect.Effect.Alignment == Alignment.Screen ? 12 : 0);

                int startWritePos = writePos;

                effect.BatchProcess((particles, begin, end) =>
                {
                    for (int i = begin; i < end; ++i)
                    {
                        var p = particles[i];

                        m_shadowedVertices1[writePos].position = p.m_position;
                        m_shadowedVertices1[writePos].expand = expand;
                        m_shadowedVertices1[writePos].size = p.m_size;
                        m_shadowedVertices1[writePos].rotation = p.m_rotation;
                        m_shadowedVertices1[writePos].uvparams = uvParams;
                        m_shadowedVertices1[writePos].color = p.m_color;
                        ++writePos;

                        m_shadowedVertices1[writePos].position = p.m_position;
                        m_shadowedVertices1[writePos].expand = expand;
                        m_shadowedVertices1[writePos].size = p.m_size;
                        m_shadowedVertices1[writePos].rotation = p.m_rotation;
                        m_shadowedVertices1[writePos].uvparams = uvParams;
                        m_shadowedVertices1[writePos].color = p.m_color;
                        ++writePos;

                        m_shadowedVertices1[writePos].position = p.m_position;
                        m_shadowedVertices1[writePos].expand = expand;
                        m_shadowedVertices1[writePos].size = p.m_size;
                        m_shadowedVertices1[writePos].rotation = p.m_rotation;
                        m_shadowedVertices1[writePos].uvparams = uvParams;
                        m_shadowedVertices1[writePos].color = p.m_color;
                        ++writePos;

                        m_shadowedVertices1[writePos].position = p.m_position;
                        m_shadowedVertices1[writePos].expand = expand;
                        m_shadowedVertices1[writePos].size = p.m_size;
                        m_shadowedVertices1[writePos].rotation = p.m_rotation;
                        m_shadowedVertices1[writePos].uvparams = uvParams;
                        m_shadowedVertices1[writePos].color = p.m_color;
                        ++writePos;
                    }
                });

                writePos = startWritePos;

                effect.BatchProcess((particles, localFrames, begin, end) =>
                {
                    if (effect.Effect.Alignment == Alignment.Local)
                    {
                        for (int i = begin; i < end; ++i)
                        {
                            var col0 = localFrames[i].Col0;
                            var col1 = localFrames[i].Col1;
                            var col2 = localFrames[i].Col2;
                            var col3 = localFrames[i].Col3;

                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                        }
                    }
                    else
                    {
                        var col0 = Vector4.UnitX;
                        var col1 = Vector4.UnitY;
                        var col2 = Vector4.UnitZ;
                        var col3 = Vector4.UnitW;
                        for (int i = begin; i < end; ++i)
                        {
                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                            m_shadowedVertices2[writePos].col0 = col0;
                            m_shadowedVertices2[writePos].col1 = col1;
                            m_shadowedVertices2[writePos].col2 = col2;
                            m_shadowedVertices2[writePos++].col3 = col3;
                        }
                    }
                });
            }
            m_vertices1.SetData(m_shadowedVertices1, 0, writePos, SetDataOptions.Discard);
            m_vertices2.SetData(m_shadowedVertices2, 0, writePos, SetDataOptions.Discard);
        }

        private void UpdateIndices()
        {
            var len = SystemInstance.TotalLiveParticles * 6;
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

            m_paramExpandMatrices.SetValue(new Matrix[] {
                new Matrix { M11 =  1, M22 =  1 },  // +X+Y
                new Matrix { M11 =  1, M22 = -1 },  // +X-Y
                new Matrix { M11 = -1, M22 =  1 },  // -X+Y
                new Matrix { M11 = -1, M22 = -1 },  // -X-Y
                new Matrix { M11 =  1, M23 =  1 },  // +X+Z
                new Matrix { M11 =  1, M23 = -1 },  // +X-Z
                new Matrix { M11 = -1, M23 =  1 },  // -X+Z
                new Matrix { M11 = -1, M23 = -1 },  // -X-Z
                new Matrix { M12 =  1, M23 =  1 },  // +Y+Z
                new Matrix { M12 =  1, M23 = -1 },  // +Y-Z
                new Matrix { M12 = -1, M23 =  1 },  // -Y+Z
                new Matrix { M12 = -1, M23 = -1 },  // -Y-Z
                Matrix.CreateScale(ProjectionMatrix.M22 / GraphicsDevice.Viewport.AspectRatio,
                                   ProjectionMatrix.M22,
                                   1) * Matrix.Invert(viewProj)
            });

            m_paramTransform.SetValue(viewProj);
            m_paramTexture.SetValue(SystemInstance.System.TextureObject ?? m_whiteTexture);
        }
    }
}
