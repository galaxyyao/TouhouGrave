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
        private struct ParticleVertex1 : IVertexType
        {
            public Vector3 position;
            public float corner;
            public Vector2 size;
            public float rotation;
            public Vector4 uvparams;
            public Particle.Color color;

            private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
                new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
                new VertexElement(28, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(44, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );

            public static int Size { get { return s_vertDecl.VertexStride; } }
            VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
        };

        private struct ParticleVertex2 : IVertexType
        {
            public Vector3 xaxis;
            public Vector3 yaxis;

            private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 2)
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
        private EffectParameter m_paramTransform;
        private EffectParameter m_paramTexture;
        private Matrix m_alignToScreen;
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

            UpdateParameters(system, transform, screenScale, aspectRatio);

            FillShadowedVertices(system);
            FillShadowedIndices();

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

            device.SetVertexBuffers(new VertexBufferBinding(m_vertices1), new VertexBufferBinding(m_vertices2));
            device.Indices = m_indices;

            var oldDepth = device.DepthStencilState;
            device.DepthStencilState = m_depthDisabled;

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
        }

        public override void Startup()
        {
            Particle.ResourceLoader.Instance = new ResourceLoader();

            CreateGeometry(BufferCapacity);

            // located in ParticleContent
            m_effect = GameApp.Service<Services.ResourceManager>().Acquire<Effect>("Effects/ParticleRenderer");
            m_effect.CurrentTechnique = m_effect.Techniques[0];

            m_paramCorners = m_effect.Parameters["Corners"];
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

        private void FillShadowedVertices(Particle.ParticleSystem system)
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

                var startWritePos = writePos;

                effect.BatchProcess((particles, begin, end) =>
                {
                    for (int i = begin; i < end; ++i)
                    {
                        var p = particles[i];

                        m_shadowedVertices1[writePos].position = p.m_position;
                        m_shadowedVertices1[writePos].size = p.m_size;
                        m_shadowedVertices1[writePos].rotation = p.m_rotation;
                        m_shadowedVertices1[writePos].uvparams = uvParams;
                        m_shadowedVertices1[writePos].color = p.m_color;
                        ++writePos;

                        m_shadowedVertices1[writePos].position = p.m_position;
                        m_shadowedVertices1[writePos].size = p.m_size;
                        m_shadowedVertices1[writePos].rotation = p.m_rotation;
                        m_shadowedVertices1[writePos].uvparams = uvParams;
                        m_shadowedVertices1[writePos].color = p.m_color;
                        ++writePos;

                        m_shadowedVertices1[writePos].position = p.m_position;
                        m_shadowedVertices1[writePos].size = p.m_size;
                        m_shadowedVertices1[writePos].rotation = p.m_rotation;
                        m_shadowedVertices1[writePos].uvparams = uvParams;
                        m_shadowedVertices1[writePos].color = p.m_color;
                        ++writePos;

                        m_shadowedVertices1[writePos].position = p.m_position;
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
                    if (effect.Alignment == Particle.Alignment.Local)
                    {
                        for (int i = begin; i < end; ++i)
                        {
                            m_shadowedVertices2[writePos].xaxis = localFrames[i].XAxis;
                            m_shadowedVertices2[writePos++].yaxis = localFrames[i].YAxis;
                            m_shadowedVertices2[writePos].xaxis = localFrames[i].XAxis;
                            m_shadowedVertices2[writePos++].yaxis = localFrames[i].YAxis;
                            m_shadowedVertices2[writePos].xaxis = localFrames[i].XAxis;
                            m_shadowedVertices2[writePos++].yaxis = localFrames[i].YAxis;
                            m_shadowedVertices2[writePos].xaxis = localFrames[i].XAxis;
                            m_shadowedVertices2[writePos++].yaxis = localFrames[i].YAxis;
                        }
                    }
                    else
                    {
                        Vector3 unitX, unitY;
                        switch (effect.Alignment)
                        {
                            case Particle.Alignment.Screen:
                                unitX.X = m_alignToScreen.M11;
                                unitX.Y = m_alignToScreen.M12;
                                unitX.Z = m_alignToScreen.M13;
                                unitY.X = m_alignToScreen.M21;
                                unitY.Y = m_alignToScreen.M22;
                                unitY.Z = m_alignToScreen.M23;
                                break;
                            default:
                            case Particle.Alignment.WorldXY:
                                unitX.X = unitY.Y = 1;
                                unitX.Y = unitX.Z = unitY.X = unitY.Z = 0;
                                break;
                            case Particle.Alignment.WorldXZ:
                                unitX.X = unitY.Z = 1;
                                unitX.Y = unitX.Z = unitY.X = unitY.Y = 0;
                                break;
                            case Particle.Alignment.WorldYZ:
                                unitX.Y = unitY.Z = 1;
                                unitX.X = unitX.Z = unitY.X = unitY.Y = 0;
                                break;
                        }
                        for (int i = begin; i < end; ++i)
                        {
                            m_shadowedVertices2[writePos].xaxis = unitX;
                            m_shadowedVertices2[writePos++].yaxis = unitY;
                            m_shadowedVertices2[writePos].xaxis = unitX;
                            m_shadowedVertices2[writePos++].yaxis = unitY;
                            m_shadowedVertices2[writePos].xaxis = unitX;
                            m_shadowedVertices2[writePos++].yaxis = unitY;
                            m_shadowedVertices2[writePos].xaxis = unitX;
                            m_shadowedVertices2[writePos++].yaxis = unitY;
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

        private void UpdateParameters(Particle.ParticleSystem system, Matrix transform, float aspectRatio, float screenScale)
        {
            m_paramCorners.SetValue(new Vector2[] {
                new Vector2(-0.5f,  0.5f),
                new Vector2( 0.5f,  0.5f),
                new Vector2(-0.5f, -0.5f),
                new Vector2( 0.5f, -0.5f),
            });

            var device = GameApp.Instance.GraphicsDevice;
            m_alignToScreen = Matrix.CreateScale(screenScale / aspectRatio, screenScale, 1) * Matrix.Invert(transform);

            m_paramTransform.SetValue(transform);
            m_paramTexture.SetValue(system.TextureObject ?? m_whiteTexture);
        }
    }
}
