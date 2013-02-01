using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Graphics
{
    [Services.LifetimeDependency(typeof(Services.ResourceManager))]
    partial class ParticleRenderer : Services.GameService
    {
        private struct ParticleVertex1 : IVertexType
        {
            public Vector3 position;
            public Byte corner;
            public Byte expand;
#pragma warning disable 649 // Field '' is never assigned to, and will always have its default value 0
            public UInt16 padding;
#pragma warning restore 649
            public HalfVector4 uvparams;
            public Vector2 size;
            public float rotation;
            public Particle.Color color;

            private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.Position, 1),
                new VertexElement(16, VertexElementFormat.HalfVector4, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Position, 2),
                new VertexElement(36, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );

            public static int Size { get { return s_vertDecl.VertexStride; } }
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

            public static int Size { get { return s_vertDecl.VertexStride; } }
            VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
        };

        private ParticleVertex1[] m_shadowedVertices1;
        private ParticleVertex2[] m_shadowedVertices2;
        private UInt16[] m_shadowedIndices;
        private Particle.ParticleSorter m_sorter = new Particle.ParticleSorter();

        private const int BufferCapacity = 500;
        private int m_bufferWriteCursor = 0;
        private DynamicVertexBuffer m_vertices1;
        private DynamicVertexBuffer m_vertices2;
        private DynamicIndexBuffer m_indices;

        private Effect m_effect;
        private EffectParameter m_paramCorners;
        private EffectParameter m_paramExpandMatrices;
        private EffectParameter m_paramTransform;
        private EffectParameter m_paramTexture;
        private Texture2D m_whiteTexture;

        private BlendState m_multiplyBlend;
        private DepthStencilState m_depthDisabled;

        public void Draw(Particle.ParticleSystemInstance instance, Matrix transform, float screenScale, float aspectRatio)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("system");
            }

            var numParticles = instance.TotalLiveParticles;
            if (numParticles == 0)
            {
                return;
            }

            m_sorter.Sort(instance, Vector3.UnitZ);

            UpdateParameters(instance, transform, screenScale, aspectRatio);

            FillShadowedVertices(instance);
            FillShadowedIndices();

            var device = GameApp.Instance.GraphicsDevice;

            var oldBlend = device.BlendState;
            switch (instance.System.BlendMode)
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

            device.SetVertexBuffers(new VertexBufferBinding(m_vertices1), new VertexBufferBinding(m_vertices2));
            device.Indices = m_indices;

            var oldDepth = device.DepthStencilState;
            device.DepthStencilState = m_depthDisabled;

            var oldRasterizer = device.RasterizerState;
            device.RasterizerState = instance.System.DoubleFaced ? RasterizerState.CullNone : RasterizerState.CullCounterClockwise;

            SetDataOptions hint = SetDataOptions.NoOverwrite;
            if (m_bufferWriteCursor + numParticles > BufferCapacity)
            {
                hint = SetDataOptions.Discard;
                m_bufferWriteCursor = 0;
            }

            var vertSize1 = ParticleVertex1.Size;
            m_vertices1.SetData(m_bufferWriteCursor * 4 * vertSize1, m_shadowedVertices1, 0, numParticles * 4, vertSize1, hint);
            var vertSize2 = ParticleVertex2.Size;
            m_vertices2.SetData(m_bufferWriteCursor * 4 * vertSize2, m_shadowedVertices2, 0, numParticles * 4, vertSize2, hint);
            m_indices.SetData(m_bufferWriteCursor * 6 * 2, m_shadowedIndices, 0, numParticles * 6, hint);

            foreach (var pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, m_bufferWriteCursor * 4, 0, numParticles * 4, m_bufferWriteCursor * 6, numParticles * 2);
            }

            m_bufferWriteCursor += numParticles;

            device.BlendState = oldBlend;
            device.DepthStencilState = oldDepth;
            device.RasterizerState = oldRasterizer;
        }

        public override void Startup()
        {
            Particle.ResourceLoader.Instance = new ResourceLoader();

            CreateGeometry(BufferCapacity);

            // located in ParticleContent
            m_effect = GameApp.Service<Services.ResourceManager>().Acquire<Effect>("Effects/ParticleRenderer");
            m_effect.CurrentTechnique = m_effect.Techniques[0];

            m_paramCorners = m_effect.Parameters["Corners"];
            m_paramExpandMatrices = m_effect.Parameters["ExpandMatrices"];
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
            m_vertices1.Dispose();
            m_vertices2.Dispose();
        }

        private void CreateGeometry(int capacity)
        {
            var device = GameApp.Instance.GraphicsDevice;
            m_vertices1 = new DynamicVertexBuffer(device, typeof(ParticleVertex1), capacity * 4, 0);
            m_vertices2 = new DynamicVertexBuffer(device, typeof(ParticleVertex2), capacity * 4, 0);
            m_indices = new DynamicIndexBuffer(device, IndexElementSize.SixteenBits, capacity * 6, 0);
        }

        private void FillShadowedVertices(Particle.ParticleSystemInstance instance)
        {
            if (m_shadowedVertices1 == null || m_shadowedVertices1.Length < m_sorter.SortedIndices.Length * 4)
            {
                m_shadowedVertices1 = new ParticleVertex1[m_sorter.SortedIndices.Length * 4];
                m_shadowedVertices2 = new ParticleVertex2[m_sorter.SortedIndices.Length * 4];
                for (int i = 0; i < m_shadowedVertices1.Length; ++i)
                {
                    m_shadowedVertices1[i].corner = (Byte)(i & 0x3);
                }
            }

            var texture = instance.System.TextureObject ?? m_whiteTexture;
            float invTexWidth = 1f / texture.Width;
            float invTexHeight = 1f / texture.Height;

            int writePos = 0;
            foreach (var effect in instance.EffectInstances)
            {
                XnaRect uvBounds = effect.Effect.UVBounds;

                var uvParamX = (float)uvBounds.Width * invTexWidth;
                var uvParamY = -(float)uvBounds.Height * invTexHeight;
                var uvParamZ = 0.5f * uvParamX + (uvBounds.X - 0.5f) * invTexWidth;
                var uvParamW = -0.5f * uvParamY + (uvBounds.Y - 0.5f) * invTexHeight;
                var uvParams = new HalfVector4(uvParamX, uvParamY, uvParamZ, uvParamW);

                var expand = (Byte)(effect.Effect.Alignment == Particle.Alignment.Screen ? 12 : 0);

                var startWritePos = writePos;

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
                    if (effect.Effect.Alignment == Particle.Alignment.Local)
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

        private void UpdateParameters(Particle.ParticleSystemInstance instance, Matrix transform, float aspectRatio, float screenScale)
        {
            m_paramCorners.SetValue(new Vector2[] {
                new Vector2(-0.5f,  0.5f),
                new Vector2( 0.5f,  0.5f),
                new Vector2(-0.5f, -0.5f),
                new Vector2( 0.5f, -0.5f),
            });

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
                Matrix.CreateScale(screenScale / aspectRatio,
                                   screenScale,
                                   1) * Matrix.Invert(transform)
            });

            m_paramTransform.SetValue(transform);
            m_paramTexture.SetValue(instance.System.TextureObject ?? m_whiteTexture);
        }
    }
}
