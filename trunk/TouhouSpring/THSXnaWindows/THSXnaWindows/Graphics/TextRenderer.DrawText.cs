using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Byte4 = Microsoft.Xna.Framework.Graphics.PackedVector.Byte4;
using SystemFont = System.Drawing.Font;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        private const int VertexBufferGranularity = 32;

        private struct PositionedGlyphData
        {
            public Vector2 m_leftTop;
            public GlyphData m_glyphData;
        }

        private struct PositionedGlyphPage
        {
            public Vector2 m_leftTop;
            public int m_pageIndex;
        }

        private struct PageInstance
        {
            public Vector2 m_leftTopPos;
            public Byte4 m_localPageXY_mask;
            public Color m_color;
        };

        private VertexDeclaration m_instanceVertDecl;
        private DynamicVertexBuffer m_instanceVertices;
        private VertexBuffer m_quadVertices;
        private IndexBuffer m_quadIndices;
        private EffectTechnique m_techDraw;
        private EffectParameter m_paramPageSizeInViewport;
        private EffectParameter m_paramPageSizeInSrc;
        private EffectParameter m_paramWorldViewProj;

        private int m_instanceBufferWriteCursor = 0;

        public void DrawText(string text, SystemFont font, Color color, Matrix transform)
        {
            var glyphs = LayoutText(text, font);
            int totalPages = glyphs.Sum(glyph => glyph.m_glyphData.m_pageIndices.Length);
            var glyphPages = new PositionedGlyphPage[totalPages];
            var pageInstances = new PageInstance[totalPages];

            int counter = 0;
            foreach (var glyph in glyphs)
            {
                var pagesInX = glyph.m_glyphData.m_pageIndices.GetUpperBound(0);
                var pagesInY = glyph.m_glyphData.m_pageIndices.GetUpperBound(1);

                for (int i = 0; i <= pagesInX; ++i)
                {
                    for (int j = 0; j <= pagesInY; ++j)
                    {
                        glyphPages[counter].m_leftTop.X = glyph.m_leftTop.X + i * PageSize;
                        glyphPages[counter].m_leftTop.Y = glyph.m_leftTop.Y + j * PageSize;
                        glyphPages[counter].m_pageIndex = glyph.m_glyphData.m_pageIndices[i, j];
                        ++counter;
                    }
                }
            }

            Func<PositionedGlyphPage, int> batchCriteria = page => page.m_pageIndex / PagesInOneCacheTexture / 4;
            counter = 0;

            var device = GameApp.Instance.GraphicsDevice;
            device.Indices = m_quadIndices;
            device.BlendState = BlendState.NonPremultiplied;
            device.DepthStencilState = DepthStencilState.None;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            m_effect.CurrentTechnique = m_techDraw;
            m_paramWorldViewProj.SetValue(transform);
            var vpWidth = (float)device.Viewport.Width;
            var vpHeight = (float)device.Viewport.Height;
            m_paramPageSizeInViewport.SetValue(new Vector2(PageSize / vpWidth * 2, -PageSize / vpHeight * 2));

            foreach (var batch in glyphPages.GroupBy(batchCriteria))
            {
                Texture2D texture = m_cacheTextures[batch.Key * 4].m_physicalRTTexture;
                m_paramTexture.SetValue(texture);
                m_paramPageSizeInSrc.SetValue(new Vector2((float)PageSize / texture.Width, (float)PageSize / texture.Height));
                var arrayStart = counter;

                foreach (var glyphPage in batch)
                {
                    var localPageIndex = glyphPage.m_pageIndex % PagesInOneCacheTexture;
                    var pageX = localPageIndex % PagesInOneRow;
                    var pageY = localPageIndex / PagesInOneRow;
                    int channel = glyphPage.m_pageIndex / PagesInOneCacheTexture % 4;

                    pageInstances[counter].m_leftTopPos.X = (glyphPage.m_leftTop.X - 0.5f) / vpWidth * 2 - 1;
                    pageInstances[counter].m_leftTopPos.Y = 1 - (glyphPage.m_leftTop.Y - 0.5f) / vpHeight * 2;
                    pageInstances[counter].m_localPageXY_mask = new Byte4(pageX, pageY, channel, 0);
                    pageInstances[counter].m_color = color;

                    ++counter;
                }

                var batchSize = counter - arrayStart;
                var instanceBufferOffset = CopyInstanceVertices(pageInstances, arrayStart, batchSize);
                device.SetVertexBuffers(m_quadVertices, new VertexBufferBinding(m_instanceVertices, instanceBufferOffset, 1));

                foreach (var pass in m_techDraw.Passes)
                {
                    pass.Apply();
                    device.DrawInstancedPrimitives(PrimitiveType.TriangleStrip, 0, 0, 4, 0, 2, batchSize);
                }
            }
        }

        private List<PositionedGlyphData> LayoutText(string text, SystemFont font)
        {
            var glyphs = new List<PositionedGlyphData>();

            // load each glyph
            int index = 0;
            foreach (var character in text)
            {
                glyphs.Add(new PositionedGlyphData
                {
                    m_leftTop = new Vector2(index * 50, 0),
                    m_glyphData = Load(character, font)
                });
                ++index;
            }

            return glyphs;
        }

        private int CopyInstanceVertices(PageInstance[] instances, int startIndex, int numElements)
        {
            int roundedLength = ((numElements - 1) / VertexBufferGranularity + 1) * VertexBufferGranularity;
            if (m_instanceVertices != null && m_instanceVertices.VertexCount < roundedLength)
            {
                m_instanceVertices.Dispose();
                m_instanceVertices = null;
            }
            if (m_instanceVertices == null)
            {
                m_instanceVertices = new DynamicVertexBuffer(GameApp.Instance.GraphicsDevice, m_instanceVertDecl, roundedLength, BufferUsage.WriteOnly);
                m_instanceBufferWriteCursor = 0;
            }

            SetDataOptions hint = SetDataOptions.NoOverwrite;
            if (m_instanceBufferWriteCursor + numElements > m_instanceVertices.VertexCount)
            {
                hint = SetDataOptions.Discard;
                m_instanceBufferWriteCursor = 0;
            }

            var vertSize = m_instanceVertDecl.VertexStride;
            var offsetInVertices = m_instanceBufferWriteCursor;
            m_instanceVertices.SetData(offsetInVertices * vertSize, instances, startIndex, numElements, vertSize, hint);
            m_instanceBufferWriteCursor += numElements;
            return offsetInVertices;
        }

        private void Initialize_DrawText()
        {
            var quadVertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
            );
            m_quadVertices = new VertexBuffer(GameApp.Instance.GraphicsDevice, quadVertDecl, 4, BufferUsage.None);
            m_quadVertices.SetData(new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            });

            m_quadIndices = new IndexBuffer(GameApp.Instance.GraphicsDevice, IndexElementSize.SixteenBits, 4, BufferUsage.None);
            m_quadIndices.SetData(new UInt16[] { 0, 1, 2, 3 });

            m_instanceVertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(8, VertexElementFormat.Byte4, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 1)
            );

            m_techDraw = m_effect.Techniques["DrawText"];
            m_paramPageSizeInViewport = m_effect.Parameters["Draw_VPPageSize"];
            m_paramPageSizeInSrc = m_effect.Parameters["Draw_SrcPageSize"];
            m_paramWorldViewProj = m_effect.Parameters["Draw_WorldViewProj"];
        }

        private void Destroy_DrawText()
        {
            if (m_instanceVertices != null)
            {
                m_instanceVertices.Dispose();
            }
            m_instanceVertDecl.Dispose();
            m_quadIndices.Dispose();
            m_quadVertices.Dispose();
        }
    }
}
