using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Byte4 = Microsoft.Xna.Framework.Graphics.PackedVector.Byte4;
using Short2N = Microsoft.Xna.Framework.Graphics.PackedVector.NormalizedShort2;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        private const int VertexBufferGranularity = 32;

        private struct PositionedGlyphPage
        {
            public Vector2 m_pos;
            public Color m_color;
            public bool m_hasOutline;
            public int m_pageIndex;
        }

        private struct VertexDataDraw
        {
            public Short2N m_corner;
            public Vector2 m_leftTopPos;
            public Byte4 m_localPageXY_mask;
            public Color m_color;
        };

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
        private EffectParameter m_paramOutlineColor;

        [Flags]
        public enum DrawFlags
        {
            TransformToClipSpace    = 0x01,
            OffsetByHalfPixel       = 0x02,
            BoundedByBox            = 0x04
        }

        public struct DrawOptions
        {
            public Color ForcedColor;
            public Vector4 ColorScaling;
            public Vector4 OutlineColor;
            public Point Offset;
            public DrawFlags DrawFlags;
            public int SubstringStart;
            public int SubstringLength;
            public int BaseIndex;
            public Rectangle BoundingBox;

            public static readonly DrawOptions Default = new DrawOptions
            {
                ForcedColor = Color.Transparent,
                ColorScaling = Vector4.UnitW,
                OutlineColor = Vector4.UnitW,
                Offset = new Point(0, 0),
                DrawFlags = TextRenderer.DrawFlags.OffsetByHalfPixel,
                SubstringStart = 0,
                SubstringLength = -1,
                BaseIndex = 0,
                BoundingBox = new Rectangle(0, 0, 0, 0)
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

        public void DrawText(IFormattedText formattedText, Matrix transform)
        {
            DrawText(formattedText, transform, DrawOptions.Default);
        }

        public void DrawText(IFormattedText formattedText, Matrix transform, DrawOptions drawOptions)
        {
            if (!(formattedText is FormattedText))
            {
                throw new ArgumentException("Argument formattedText is not an object returned by FormatText() method.");
            }

            PInvokes.D3d9.BeginPixEvent(0, "DrawText: " + formattedText.Text);

            var typedFormattedText = (FormattedText)formattedText;
            if ((typedFormattedText.RichTextFormat || typedFormattedText.m_lines.Length > 1 || typedFormattedText.FormatOptions.Alignment != Alignment.LeftTop)
                && (drawOptions.SubstringStart != 0 || drawOptions.SubstringLength != -1 || drawOptions.BaseIndex != 0))
            {
                throw new InvalidOperationException("Drawing substring can only be performed on non-RTF text.");
            }

            var globalOffset = Vector2.Zero;
            globalOffset.X = typedFormattedText.Offset.X + drawOptions.Offset.X;
            globalOffset.Y = typedFormattedText.Offset.Y + drawOptions.Offset.Y;
            if ((drawOptions.DrawFlags & DrawFlags.OffsetByHalfPixel) != 0)
            {
                globalOffset += new Vector2(-0.5f, -0.5f);
            }

            List<PositionedGlyphPage> glyphPages = new List<PositionedGlyphPage>();
            foreach (var line in typedFormattedText.m_lines)
            {
                for (int i = 0; i < (drawOptions.SubstringLength == -1 ? line.m_glyphs.Length : drawOptions.SubstringLength); ++i)
                {
                    var glyph = line.m_glyphs[i + drawOptions.SubstringStart];

                    if (IsWhitespaceChar(glyph.m_glyph))
                    {
                        continue;
                    }

                    var glyphData = Load(glyph.m_glyph, typedFormattedText.FormatOptions);
                    var pagesInX = glyphData.m_pageIndices.GetUpperBound(0);
                    var pagesInY = glyphData.m_pageIndices.GetUpperBound(1);

                    for (int x = 0; x <= pagesInX; ++x)
                    {
                        for (int y = 0; y <= pagesInY; ++y)
                        {
                            var glyphPos = glyph.m_pos + line.m_offset + globalOffset;
                            glyphPos.X -= line.m_glyphs[drawOptions.BaseIndex].m_pos.X;
                            glyphPages.Add(new PositionedGlyphPage
                            {
                                m_pos = glyphPos + new Vector2(x * PageSize, y * PageSize),
                                m_color = drawOptions.ForcedColor == Color.Transparent ? glyph.m_color : drawOptions.ForcedColor,
                                m_hasOutline = IsAnsiChar(glyph.m_glyph)
                                               ? formattedText.FormatOptions.AnsiFont.OutlineThickness > 0
                                               : formattedText.FormatOptions.Font.OutlineThickness > 0,
                                m_pageIndex = glyphData.m_pageIndices[x, y]
                            });
                        }
                    }
                }
            }

            Func<PositionedGlyphPage, int> batchCriteria = page => page.m_pageIndex / PagesInOneCacheTexture / 4;
            bool useBoundingBox = (drawOptions.DrawFlags & DrawFlags.BoundedByBox) != 0;
            int counter = 0;

            var device = GameApp.Instance.GraphicsDevice;
            device.Indices = null;
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.None;
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            m_effect.CurrentTechnique = m_techDraw;
            if ((drawOptions.DrawFlags & DrawFlags.TransformToClipSpace) != 0)
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
            var premultipliedOutlineColor = drawOptions.OutlineColor;
            premultipliedOutlineColor.X *= premultipliedOutlineColor.W;
            premultipliedOutlineColor.Y *= premultipliedOutlineColor.W;
            premultipliedOutlineColor.Z *= premultipliedOutlineColor.W;
            m_paramOutlineColor.SetValue(premultipliedOutlineColor);

            var vertices = new VertexDataDraw[glyphPages.Count * 6];

            foreach (var batch in glyphPages.GroupBy(batchCriteria))
            {
                Texture2D texture = m_cacheTextures[batch.Key * 4].m_physicalRTTexture;
                m_paramTexture.SetValue(texture);

                var arrayStart = counter;

                foreach (var glyphPage in batch)
                {
                    float boundedLeft, boundedTop, boundedWidth, boundedHeight;
                    if (useBoundingBox)
                    {
                        boundedLeft = (drawOptions.BoundingBox.Left - glyphPage.m_pos.X) / PageSize;
                        boundedTop = (drawOptions.BoundingBox.Top - glyphPage.m_pos.Y) / PageSize;
                        boundedWidth = (drawOptions.BoundingBox.Right - glyphPage.m_pos.X) / PageSize;
                        boundedHeight = (drawOptions.BoundingBox.Bottom - glyphPage.m_pos.Y) / PageSize;

                        if (boundedLeft >= 1.0f || boundedTop >= 1.0f || boundedWidth <= 0.0f || boundedHeight <= 0.0f)
                        {
                            continue;
                        }
                        boundedLeft = Math.Max(boundedLeft, 0.0f);
                        boundedTop = Math.Max(boundedTop, 0.0f);
                        boundedWidth = Math.Min(boundedWidth, 1.0f);
                        boundedHeight = Math.Min(boundedHeight, 1.0f);
                    }
                    else
                    {
                        boundedLeft = boundedTop = 0;
                        boundedWidth = boundedHeight = 1;
                    }

                    var localPageIndex = glyphPage.m_pageIndex % PagesInOneCacheTexture;
                    var pageX = localPageIndex % PagesInOneRow;
                    var pageY = localPageIndex / PagesInOneRow;
                    int channel = glyphPage.m_pageIndex / PagesInOneCacheTexture % 4;

                    for (int i = 0; i < 6; ++i)
                    {
                        vertices[counter + i].m_leftTopPos.X = glyphPage.m_pos.X;
                        vertices[counter + i].m_leftTopPos.Y = glyphPage.m_pos.Y;
                        vertices[counter + i].m_localPageXY_mask = new Byte4(pageX, pageY, channel, glyphPage.m_hasOutline ? 1 : 0);
                        vertices[counter + i].m_color = glyphPage.m_color;
                    }
                    vertices[counter + 0].m_corner = new Short2N(boundedLeft, boundedTop);
                    vertices[counter + 1].m_corner = vertices[counter + 4].m_corner = new Short2N(boundedWidth, boundedTop);
                    vertices[counter + 2].m_corner = vertices[counter + 3].m_corner = new Short2N(boundedLeft, boundedHeight);
                    vertices[counter + 5].m_corner = new Short2N(boundedWidth, boundedHeight);

                    counter += 6;
                }

                if (counter == 0)
                {
                    break;
                }

                var batchSize = counter - arrayStart;
                var bufferOffset = CopyInstanceVertices(vertices, arrayStart, batchSize);
                device.SetVertexBuffer(m_verticesDraw);

                foreach (var pass in m_techDraw.Passes)
                {
                    pass.Apply();
                    device.DrawPrimitives(PrimitiveType.TriangleList, bufferOffset, counter / 3);
                }
            }

            PInvokes.D3d9.EndPixEvent();
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
                new VertexElement(0, VertexElementFormat.NormalizedShort2, VertexElementUsage.Position, 0),
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
            m_paramOutlineColor = m_effect.Parameters["Draw_OutlineColor"];
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
