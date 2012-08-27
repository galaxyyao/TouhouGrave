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

        private struct VertexDataDraw
        {
            public Byte4 m_corner;
            public Vector2 m_leftTopPos;
            public Byte4 m_localPageXY_mask;
            public Color m_color;
        };

        private VertexDeclaration m_vertDeclDraw;
        private DynamicVertexBuffer m_verticesDraw;
        private int m_verticesDrawWriteCursor = 0;
        private EffectTechnique m_techDraw;
        private EffectParameter m_paramPageSizeInViewport;
        private EffectParameter m_paramPageSizeInSrc;
        private EffectParameter m_paramWorldViewProj;

        public void DrawText(string text, SystemFont font, Color color, Matrix transform)
        {
            var glyphs = LayoutText(text, font);
            int totalPages = glyphs.Sum(glyph => glyph.m_glyphData.m_pageIndices.Length);
            var glyphPages = new PositionedGlyphPage[totalPages];

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
            device.Indices = null;
            device.BlendState = BlendState.NonPremultiplied;
            device.DepthStencilState = DepthStencilState.None;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            m_effect.CurrentTechnique = m_techDraw;
            m_paramWorldViewProj.SetValue(transform);
            var vpWidth = (float)device.Viewport.Width;
            var vpHeight = (float)device.Viewport.Height;
            m_paramPageSizeInViewport.SetValue(new Vector2(PageSize / vpWidth * 2, -PageSize / vpHeight * 2));

            var vertices = new VertexDataDraw[totalPages * 6];

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

                    for (int i = 0; i < 6; ++i)
                    {
                        vertices[counter + i].m_leftTopPos.X = (glyphPage.m_leftTop.X - 0.5f) / vpWidth * 2 - 1;
                        vertices[counter + i].m_leftTopPos.Y = 1 - (glyphPage.m_leftTop.Y - 0.5f) / vpHeight * 2;
                        vertices[counter + i].m_localPageXY_mask = new Byte4(pageX, pageY, channel, 0);
                        vertices[counter + i].m_color = color;
                    }
                    vertices[counter + 0].m_corner = new Byte4(0, 0, 0, 0);
                    vertices[counter + 1].m_corner = vertices[counter + 4].m_corner = new Byte4(1, 0, 0, 0);
                    vertices[counter + 2].m_corner = vertices[counter + 3].m_corner = new Byte4(0, 1, 0, 0);
                    vertices[counter + 5].m_corner = new Byte4(1, 1, 0, 0);

                    counter += 6;
                }

                var batchSize = counter - arrayStart;
                var bufferOffset = CopyInstanceVertices(vertices, arrayStart, batchSize);
                device.SetVertexBuffer(m_verticesDraw, bufferOffset);

                foreach (var pass in m_techDraw.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, counter / 3);
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

        private int CopyInstanceVertices(VertexDataDraw[] vertices, int startIndex, int numElements)
        {
            int roundedLength = ((numElements - 1) / VertexBufferGranularity + 1) * VertexBufferGranularity;
            if (m_verticesDraw != null && m_verticesDraw.VertexCount < roundedLength)
            {
                m_verticesDraw.Dispose();
                m_verticesDraw = null;
            }
            if (m_verticesDraw == null)
            {
                m_verticesDraw = new DynamicVertexBuffer(GameApp.Instance.GraphicsDevice, m_vertDeclDraw, roundedLength, BufferUsage.WriteOnly);
                m_verticesDrawWriteCursor = 0;
            }

            SetDataOptions hint = SetDataOptions.NoOverwrite;
            if (m_verticesDrawWriteCursor + numElements > m_verticesDraw.VertexCount)
            {
                hint = SetDataOptions.Discard;
                m_verticesDrawWriteCursor = 0;
            }

            var vertSize = m_vertDeclDraw.VertexStride;
            var offsetInVertices = m_verticesDrawWriteCursor;
            m_verticesDraw.SetData(offsetInVertices * vertSize, vertices, startIndex, numElements, vertSize, hint);
            m_verticesDrawWriteCursor += numElements;
            return offsetInVertices;
        }

        private void Initialize_DrawText()
        {
            m_vertDeclDraw = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.Position, 0),
                new VertexElement(4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.Color, 0),
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 1)
            );

            m_techDraw = m_effect.Techniques["DrawText"];
            m_paramPageSizeInViewport = m_effect.Parameters["Draw_VPPageSize"];
            m_paramPageSizeInSrc = m_effect.Parameters["Draw_SrcPageSize"];
            m_paramWorldViewProj = m_effect.Parameters["Draw_WorldViewProj"];
        }

        private void Destroy_DrawText()
        {
            if (m_verticesDraw != null)
            {
                m_verticesDraw.Dispose();
            }
            m_vertDeclDraw.Dispose();
        }
    }
}
