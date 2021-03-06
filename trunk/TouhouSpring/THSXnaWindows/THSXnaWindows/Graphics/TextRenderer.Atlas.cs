﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MemoryStream = System.IO.MemoryStream;
using SystemDrawing = System.Drawing;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        private const int PagesInOneRow = CacheTextureSize / PageSize;
        private const int RowsInOneCacheTexture = CacheTextureSize / PageSize;
        private const int PagesInOneCacheTexture = PagesInOneRow * RowsInOneCacheTexture;

        private class GlyphData
        {
            public SystemDrawing.SizeF m_glyphSize;
            public int[,] m_pageIndices; // a glyph could occupy multiple pages
            public int m_timeStamp;
        }

        private struct GlyphPage
        {
            public int m_x;
            public int m_y;
            public int m_pageIndex;
        }

        private class CacheTexture
        {
            public RenderTarget2D m_physicalRTTexture;
            public bool[] m_pageOccupied = new bool[PagesInOneCacheTexture];
        }

        private struct VertexDataBlit : IVertexType
        {
            public Vector2 pos;
            public Vector2 uv;

            private static readonly VertexDeclaration s_vertDecl = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

            VertexDeclaration IVertexType.VertexDeclaration { get { return s_vertDecl; } }
        }

        private Dictionary<uint, GlyphData> m_loadedGlyphs = new Dictionary<uint, GlyphData>();
        private List<CacheTexture> m_cacheTextures = new List<CacheTexture>();

        private SystemDrawing.SolidBrush m_whiteBrush;
        private Dictionary<float, SystemDrawing.Pen> m_outlinePensWithWidths = new Dictionary<float, SystemDrawing.Pen>();
        private SystemDrawing.Graphics m_measureContext;
        private SystemDrawing.StringFormat m_measureFormat;
        private EffectTechnique m_techBlit;
        private BlendState[] m_channelMasks;

        // load one single glyph into the cache
        private GlyphData Load(char glyph, FormatOptions formatOptions)
        {
            int fontId = GetFontId(IsAnsiChar(glyph) ? formatOptions.AnsiFont : formatOptions.Font);
            var font = m_registeredFonts[fontId];
            uint glyphId = ((uint)fontId << 16) + glyph;

            GlyphData glyphData;
            if (m_loadedGlyphs.TryGetValue(glyphId, out glyphData))
            {
                glyphData.m_timeStamp = m_timeStamp;
                return glyphData;
            }

            var str = glyph.ToString();
            var chRect = MeasureCharacter(glyph, m_registeredFonts[fontId].m_fontObject);
            chRect.Inflate(font.m_outlineThickness * 0.5f, font.m_outlineThickness * 0.5f);

            int width = Math.Max((int)Math.Ceiling(chRect.Width), 1);
            int height = Math.Max((int)Math.Ceiling(chRect.Height), 1);
            int pagesInX = (width - 1) / PageSize + 1;
            int pagesInY = (height - 1) / PageSize + 1;

            GlyphPage[] pages = new GlyphPage[pagesInX * pagesInY];
            for (int i = 0; i < pages.Length; ++i)
            {
                pages[i].m_x = i % pagesInX;
                pages[i].m_y = i / pagesInX;
                pages[i].m_pageIndex = RequestPage();
            }

            using (var bmp = new SystemDrawing.Bitmap(pagesInX * PageSize, pagesInY * PageSize))
            using (var g = SystemDrawing.Graphics.FromImage(bmp))
            using (var memStream = new MemoryStream())
            {
                // draw text using GDI+
                g.TextRenderingHint = SystemDrawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.SmoothingMode = SystemDrawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = SystemDrawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                if (font.m_outlineThickness > 0)
                {
                    SystemDrawing.Pen outlinePen;
                    if (!m_outlinePensWithWidths.TryGetValue(font.m_outlineThickness, out outlinePen))
                    {
                        outlinePen = new SystemDrawing.Pen(SystemDrawing.Color.Gray, font.m_outlineThickness);
                        outlinePen.MiterLimit = font.m_outlineThickness;
                        m_outlinePensWithWidths.Add(font.m_outlineThickness, outlinePen);
                    }

                    // draw outline
                    using (var outlinePath = new SystemDrawing.Drawing2D.GraphicsPath())
                    {
                        outlinePath.AddString(str,
                            font.m_fontObject.FontFamily,
                            (int)font.m_fontObject.Style,
                            g.DpiX * font.m_fontObject.SizeInPoints / 72,
                            new SystemDrawing.PointF(-chRect.Left, -chRect.Top),
                            SystemDrawing.StringFormat.GenericDefault);
                        g.DrawPath(outlinePen, outlinePath);
                        g.FillPath(m_whiteBrush, outlinePath);
                    }
                }
                else
                {
                    g.DrawString(str, font.m_fontObject, m_whiteBrush, new SystemDrawing.PointF(-chRect.Left, -chRect.Top));
                }

                bmp.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
                using (var tmpTexture = Texture2D.FromStream(GameApp.Instance.GraphicsDevice, memStream))
                {
                    var device = GameApp.Instance.GraphicsDevice;
                    device.DepthStencilState = DepthStencilState.None;
                    device.RasterizerState = RasterizerState.CullCounterClockwise;
                    device.Indices = null;

                    m_effect.CurrentTechnique = m_techBlit;
                    m_paramTexture.SetValue(tmpTexture);

                    foreach (var batch in pages.GroupBy(page => page.m_pageIndex / PagesInOneCacheTexture))
                    {
                        var textureId = batch.Key;
                        device.SetRenderTarget(m_cacheTextures[textureId].m_physicalRTTexture);
                        device.BlendState = m_channelMasks[textureId % 4];

                        var pagesInBatch = batch.ToArray();
                        var vertices = new VertexDataBlit[pagesInBatch.Length * 6];
                        for (int i = 0; i < pagesInBatch.Length; ++i)
                        {
                            var page = pagesInBatch[i];
                            var dstRectLeft = (page.m_pageIndex % PagesInOneCacheTexture) % PagesInOneRow * PageSize;
                            var dstRectTop = (page.m_pageIndex % PagesInOneCacheTexture) / PagesInOneRow * PageSize;

                            float posLeft = (dstRectLeft - 0.5f) / CacheTextureSize * 2 - 1;
                            float posTop = 1 - (dstRectTop - 0.5f) / CacheTextureSize * 2;
                            float posWidth = PageSize / (float)CacheTextureSize * 2;
                            float posHeight = -PageSize / (float)CacheTextureSize * 2;

                            float uvLeft = page.m_x / (float)pagesInX;
                            float uvTop = page.m_y / (float)pagesInY;
                            float uvWidth = 1.0f / pagesInX;
                            float uvHeight = 1.0f / pagesInY;

                            // left-top
                            vertices[i * 6 + 0].pos = new Vector2(posLeft, posTop);
                            vertices[i * 6 + 0].uv = new Vector2(uvLeft, uvTop);

                            // right-top
                            vertices[i * 6 + 1].pos = vertices[i * 6 + 4].pos = new Vector2(posLeft + posWidth, posTop);
                            vertices[i * 6 + 1].uv = vertices[i * 6 + 4].uv = new Vector2(uvLeft + uvWidth, uvTop);

                            // left-bottom
                            vertices[i * 6 + 2].pos = vertices[i * 6 + 3].pos = new Vector2(posLeft, posTop + posHeight);
                            vertices[i * 6 + 2].uv = vertices[i * 6 + 3].uv = new Vector2(uvLeft, uvTop + uvHeight);

                            // right-bottom
                            vertices[i * 6 + 5].pos = new Vector2(posLeft + posWidth, posTop + posHeight);
                            vertices[i * 6 + 5].uv = new Vector2(uvLeft + uvWidth, uvTop + uvHeight);
                        }

                        foreach (var pass in m_techBlit.Passes)
                        {
                            pass.Apply();
                            device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, pagesInBatch.Length * 2);
                        }
                    }

                    device.SetRenderTarget(null);
                }
            }

            glyphData = new GlyphData();
            glyphData.m_pageIndices = new int[pagesInX, pagesInY];
            pages.ForEach(page => glyphData.m_pageIndices[page.m_x, page.m_y] = page.m_pageIndex);
            glyphData.m_glyphSize = chRect.Size;
            glyphData.m_timeStamp = m_timeStamp;
            m_loadedGlyphs.Add(glyphId, glyphData);
            return glyphData;
        }

        private int RequestPage()
        {
            for (int i = 0; i < m_cacheTextures.Count; ++i)
            {
                var ct = m_cacheTextures[i];
                for (int j = 0; j < ct.m_pageOccupied.Length; ++j)
                {
                    if (!ct.m_pageOccupied[j])
                    {
                        ct.m_pageOccupied[j] = true;
                        return i * PagesInOneCacheTexture + j;
                    }
                }
            }

            // find the least used glyph
            KeyValuePair<uint, GlyphData> leastUsedGlyph = new KeyValuePair<uint,GlyphData>(0, null);
            foreach (var kvp in m_loadedGlyphs)
            {
                if (leastUsedGlyph.Value == null
                    || kvp.Value.m_timeStamp < leastUsedGlyph.Value.m_timeStamp)
                {
                    leastUsedGlyph = kvp;
                }
            }

            if (leastUsedGlyph.Value == null || leastUsedGlyph.Value.m_timeStamp >= m_timeStamp - MinimalPageLife)
            {
                var cacheTexture = new CacheTexture();
                cacheTexture.m_physicalRTTexture = m_cacheTextures.Count % 4 == 0
                                                   ? new RenderTarget2D(GameApp.Instance.GraphicsDevice, CacheTextureSize, CacheTextureSize, true, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents)
                                                   : m_cacheTextures.Last().m_physicalRTTexture;
                cacheTexture.m_pageOccupied[0] = true;
                m_cacheTextures.Add(cacheTexture);
                return (m_cacheTextures.Count - 1) * PagesInOneCacheTexture;
            }
            else
            {
                // release the glyph
                foreach (var pageIndex in leastUsedGlyph.Value.m_pageIndices)
                {
                    var textureIndex = pageIndex / PagesInOneCacheTexture;
                    var localIndex = pageIndex % PagesInOneCacheTexture;
                    m_cacheTextures[textureIndex].m_pageOccupied[localIndex] = false;
                }
                var retIndex = leastUsedGlyph.Value.m_pageIndices[0, 0];
                m_cacheTextures[retIndex / PagesInOneCacheTexture].m_pageOccupied[retIndex % PagesInOneCacheTexture] = true;
                m_loadedGlyphs.Remove(leastUsedGlyph.Key);
                return retIndex;
            }
        }

        private SystemDrawing.RectangleF MeasureCharacter(char character, SystemDrawing.Font font)
        {
            var chRegion = m_measureContext.MeasureCharacterRanges(character.ToString(), font, new SystemDrawing.RectangleF(0, 0, 0, 0), m_measureFormat);
            return chRegion[0].GetBounds(m_measureContext);
        }

        private SystemDrawing.RectangleF MeasureSpace(SystemDrawing.Font font)
        {
            m_measureFormat.FormatFlags |= SystemDrawing.StringFormatFlags.MeasureTrailingSpaces;
            var ret = MeasureCharacter(' ', font);
            m_measureFormat.FormatFlags &= ~SystemDrawing.StringFormatFlags.MeasureTrailingSpaces;
            return ret;
        }

        private SystemDrawing.RectangleF MeasureFullwidthSpace(SystemDrawing.Font font)
        {
            m_measureFormat.FormatFlags |= SystemDrawing.StringFormatFlags.MeasureTrailingSpaces;
            var ret = MeasureCharacter('\x3000', font);
            m_measureFormat.FormatFlags &= ~SystemDrawing.StringFormatFlags.MeasureTrailingSpaces;
            return ret;
        }

        private void Initialize_Atlas()
        {
            using (var empty = new SystemDrawing.Bitmap(1, 1))
            {
                m_measureContext = SystemDrawing.Graphics.FromImage(empty);
                m_measureContext.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }

            m_whiteBrush = new SystemDrawing.SolidBrush(SystemDrawing.Color.White);
            m_measureFormat = new SystemDrawing.StringFormat(SystemDrawing.StringFormatFlags.NoClip | SystemDrawing.StringFormatFlags.NoWrap);
            m_measureFormat.SetMeasurableCharacterRanges(new SystemDrawing.CharacterRange[] { new SystemDrawing.CharacterRange(0, 1) });

            m_techBlit = m_effect.Techniques["BlitToRT"];

            m_channelMasks = new BlendState[]
            {
                new BlendState { ColorWriteChannels = ColorWriteChannels.Alpha },
                new BlendState { ColorWriteChannels = ColorWriteChannels.Red },
                new BlendState { ColorWriteChannels = ColorWriteChannels.Green },
                new BlendState { ColorWriteChannels = ColorWriteChannels.Blue }
            };
        }

        private void Destroy_Atlas()
        {
            foreach (var pen in m_outlinePensWithWidths.Values)
            {
                pen.Dispose();
            }

            m_channelMasks.ForEach(channel => channel.Dispose());
            m_whiteBrush.Dispose();
            m_measureContext.Dispose();
        }

        private void PreDeviceReset_Atlas()
        {
            // when device is going to be reset, all render targets will lose their content;
            // so unload all glyphs here; they'll be reloaded if they are drawn again
            m_loadedGlyphs.Clear();
            for (int i = 0; i < m_cacheTextures.Count; ++i)
            {
                var tex = new CacheTexture();
                tex.m_physicalRTTexture = m_cacheTextures[i].m_physicalRTTexture;
                m_cacheTextures[i] = tex;
            }
        }
    }
}
