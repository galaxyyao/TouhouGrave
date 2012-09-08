using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Byte4 = Microsoft.Xna.Framework.Graphics.PackedVector.Byte4;
using SystemColor = System.Drawing.Color;
using SystemFont = System.Drawing.Font;
using SystemSolidBrush = System.Drawing.SolidBrush;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        private const int VertexBufferGranularity = 32;

        private struct PositionedGlyphPage
        {
            public Vector2 m_pos;
            public Color m_color;
            public int m_pageIndex;
        }

        private struct VertexDataDraw
        {
            public Byte4 m_corner;
            public Vector2 m_leftTopPos;
            public Byte4 m_localPageXY_mask;
            public Color m_color;
        };

        private SystemSolidBrush m_whiteBrush;
        private VertexDeclaration m_vertDeclDraw;
        private DynamicVertexBuffer m_verticesDraw;
        private int m_verticesDrawWriteCursor = 0;
        private EffectTechnique m_techDraw;
        private EffectParameter m_paramPageSize;
        private EffectParameter m_paramPageUVSize;
        private EffectParameter m_paramWorldViewProj;
        private EffectParameter m_paramTextureSize;
        private EffectParameter m_paramInvTextureSize;
        private EffectParameter m_paramNumPages;
        private EffectParameter m_paramInvNumPages;
        private EffectParameter m_paramColorScaling;

        public struct DrawOptions
        {
            public Color ForcedColor;
            public Vector4 ColorScaling;
            public Point Offset;
            public bool TransformToClipSpace;
            public bool OffsetByHalfPixel;

            public static readonly DrawOptions Default = new DrawOptions
            {
                ForcedColor = Color.Transparent,
                ColorScaling = Vector4.UnitW,
                Offset = new Point(0, 0),
                TransformToClipSpace = false,
                OffsetByHalfPixel = true
            };
        }

        public void DrawText(string text, FormatOptions formatOptions, Matrix transform)
        {
            DrawText(FormatText(text, formatOptions), transform, DrawOptions.Default);
        }

        public void DrawText(string text, FormatOptions formatOptions, Matrix transform, DrawOptions drawOptions)
        {
            DrawText(FormatText(text, formatOptions), transform, drawOptions);
        }

        public void DrawText(IFormatedText formatedText, Matrix transform)
        {
            DrawText(formatedText, transform, DrawOptions.Default);
        }

        public void DrawText(IFormatedText formatedText, Matrix transform, DrawOptions drawOptions)
        {
            if (!(formatedText is FormatedText))
            {
                throw new ArgumentException("Argument formatedText is not an object returned by FormatText() method.");
            }

            var typedFormatedText = (FormatedText)formatedText;
            var glyphDatas = typedFormatedText.Glyphs().Select(glyph => Load(glyph.m_glyph, typedFormatedText.FormatOptions.Font)).ToArray();
            int totalPages = glyphDatas.Sum(glyph => glyph.m_pageIndices.Length);
            var glyphPages = new PositionedGlyphPage[totalPages];

            var globalOffset = Vector2.Zero;
            globalOffset.X = typedFormatedText.Offset.X + drawOptions.Offset.X;
            globalOffset.Y = typedFormatedText.Offset.Y + drawOptions.Offset.Y;
            if (drawOptions.OffsetByHalfPixel)
            {
                globalOffset += new Vector2(-0.5f, -0.5f);
            }

            int pageCounter = 0, glyphCounter = 0;
            foreach (var line in typedFormatedText.m_lines)
            {
                foreach (var glyph in line.m_glyphs)
                {
                    var pagesInX = glyphDatas[glyphCounter].m_pageIndices.GetUpperBound(0);
                    var pagesInY = glyphDatas[glyphCounter].m_pageIndices.GetUpperBound(1);

                    for (int i = 0; i <= pagesInX; ++i)
                    {
                        for (int j = 0; j <= pagesInY; ++j)
                        {
                            var glyphPos = glyph.m_pos + line.m_offset + globalOffset;
                            glyphPages[pageCounter].m_pos.X = glyphPos.X + i * PageSize;
                            glyphPages[pageCounter].m_pos.Y = glyphPos.Y + j * PageSize;
                            glyphPages[pageCounter].m_color = drawOptions.ForcedColor == Color.Transparent ? glyph.m_color : drawOptions.ForcedColor;
                            glyphPages[pageCounter].m_pageIndex = glyphDatas[glyphCounter].m_pageIndices[i, j];
                            ++pageCounter;
                        }
                    }
                    ++glyphCounter;
                }
            }

            Func<PositionedGlyphPage, int> batchCriteria = page => page.m_pageIndex / PagesInOneCacheTexture / 4;
            int counter = 0;

            var device = GameApp.Instance.GraphicsDevice;
            device.Indices = null;
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.None;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            m_effect.CurrentTechnique = m_techDraw;
            if (drawOptions.TransformToClipSpace)
            {
                var mtx = Matrix.Identity;
                mtx.M11 = 2.0f / device.Viewport.Width;
                mtx.M22 = -2.0f / device.Viewport.Height;
                mtx.M41 = -1.0f;
                mtx.M42 = 1.0f;
                transform = mtx * transform;
            }
            m_paramWorldViewProj.SetValue(transform);
            m_paramPageSize.SetValue(new Vector2(PageSize, PageSize));
            m_paramPageUVSize.SetValue(new Vector2((float)PageSize / CacheTextureSize, (float)PageSize / CacheTextureSize));
            m_paramTextureSize.SetValue(new Vector2(CacheTextureSize, CacheTextureSize));
            m_paramInvTextureSize.SetValue(new Vector2(1.0f / CacheTextureSize, 1.0f / CacheTextureSize));
            m_paramNumPages.SetValue(new Vector2(PagesInOneRow, RowsInOneCacheTexture));
            m_paramInvNumPages.SetValue(new Vector2(1.0f / PagesInOneRow, 1.0f / RowsInOneCacheTexture));
            var premultipliedColorScaling = drawOptions.ColorScaling;
            premultipliedColorScaling.X *= premultipliedColorScaling.W;
            premultipliedColorScaling.Y *= premultipliedColorScaling.W;
            premultipliedColorScaling.Z *= premultipliedColorScaling.W;
            m_paramColorScaling.SetValue(premultipliedColorScaling);

            var vertices = new VertexDataDraw[totalPages * 6];

            foreach (var batch in glyphPages.GroupBy(batchCriteria))
            {
                Texture2D texture = m_cacheTextures[batch.Key * 4].m_physicalRTTexture;
                m_paramTexture.SetValue(texture);

                var arrayStart = counter;

                foreach (var glyphPage in batch)
                {
                    var localPageIndex = glyphPage.m_pageIndex % PagesInOneCacheTexture;
                    var pageX = localPageIndex % PagesInOneRow;
                    var pageY = localPageIndex / PagesInOneRow;
                    int channel = glyphPage.m_pageIndex / PagesInOneCacheTexture % 4;

                    for (int i = 0; i < 6; ++i)
                    {
                        vertices[counter + i].m_leftTopPos.X = glyphPage.m_pos.X;
                        vertices[counter + i].m_leftTopPos.Y = glyphPage.m_pos.Y;
                        vertices[counter + i].m_localPageXY_mask = new Byte4(pageX, pageY, channel, 0);
                        vertices[counter + i].m_color = glyphPage.m_color;
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
            m_whiteBrush = new SystemSolidBrush(SystemColor.White);

            m_vertDeclDraw = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.Position, 0),
                new VertexElement(4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(12, VertexElementFormat.Byte4, VertexElementUsage.Color, 0),
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 1)
            );

            m_techDraw = m_effect.Techniques["DrawText"];
            m_paramPageSize = m_effect.Parameters["Draw_PageSize"];
            m_paramPageUVSize = m_effect.Parameters["Draw_PageUVSize"];
            m_paramWorldViewProj = m_effect.Parameters["Draw_WorldViewProj"];
            m_paramTextureSize = m_effect.Parameters["Draw_TextureSize"];
            m_paramInvTextureSize = m_effect.Parameters["Draw_InvTextureSize"];
            m_paramNumPages = m_effect.Parameters["Draw_NumPages"];
            m_paramInvNumPages = m_effect.Parameters["Draw_InvNumPages"];
            m_paramColorScaling = m_effect.Parameters["Draw_ColorScaling"];
        }

        private void Destroy_DrawText()
        {
            if (m_verticesDraw != null)
            {
                m_verticesDraw.Dispose();
            }
            m_vertDeclDraw.Dispose();

            m_whiteBrush.Dispose();
        }
    }
}
