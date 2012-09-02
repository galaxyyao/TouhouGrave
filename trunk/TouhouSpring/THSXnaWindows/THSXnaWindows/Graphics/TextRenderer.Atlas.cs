using System;
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
        }

        private struct GlyphPage
        {
            public int m_x;
            public int m_y;
            public int m_pageIndex;
        }

        private struct PageData
        {
            public Rectangle m_effectiveRegion;
            public int m_timeStamp;
        }

        private class CacheTexture
        {
            public RenderTarget2D m_physicalRTTexture;
            public PageData[] m_pages = new PageData[PagesInOneCacheTexture];
        }

        private struct VertexDataBlit
        {
            public Vector2 pos;
            public Vector2 uv;
        }

        private Dictionary<uint, GlyphData> m_loadedGlyphs = new Dictionary<uint, GlyphData>();
        private List<CacheTexture> m_cacheTextures = new List<CacheTexture>();

        private SystemDrawing.Graphics m_measureContext;
        private SystemDrawing.StringFormat m_measureFormat;
        private VertexDeclaration m_vertDeclBlit;
        private EffectTechnique m_techBlit;
        private BlendState[] m_channelMasks;

        // load one single glyph into the cache
        private GlyphData Load(char glyph, SystemDrawing.Font font)
        {
            uint glyphId = ((uint)GetFontId(font) << 16) + glyph;

            GlyphData glyphData;
            if (m_loadedGlyphs.TryGetValue(glyphId, out glyphData))
            {
                foreach (var page in glyphData.m_pageIndices)
                {
                    TouchPage(page);
                }
                return glyphData;
            }

            var str = glyph.ToString();
            var chRect = MeasureCharacter(glyph, font);

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
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.DrawString(str, font, m_whiteBrush, new SystemDrawing.PointF(-chRect.Left, -chRect.Top));
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
                            device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, pagesInBatch.Length * 2, m_vertDeclBlit);
                        }
                    }

                    device.SetRenderTarget(null);
                }
            }

            glyphData = new GlyphData();
            glyphData.m_pageIndices = new int[pagesInX, pagesInY];
            pages.ForEach(page => glyphData.m_pageIndices[page.m_x, page.m_y] = page.m_pageIndex);
            glyphData.m_glyphSize = chRect.Size;
            m_loadedGlyphs.Add(glyphId, glyphData);
            return glyphData;
        }

        private int RequestPage()
        {
            int oldestPageTime = m_timeStamp + 1;
            int oldestPageIndex = 0;

            for (int i = 0; i < m_cacheTextures.Count; ++i)
            {
                var cacheTexture = m_cacheTextures[i];
                for (int j = 0; j < cacheTexture.m_pages.Length; ++j)
                {
                    var page = cacheTexture.m_pages[j];
                    if (page.m_timeStamp < oldestPageTime)
                    {
                        oldestPageTime = page.m_timeStamp;
                        oldestPageIndex = i * PagesInOneCacheTexture + j;
                    }
                }
            }

            if (oldestPageTime >= m_timeStamp - MinimalPageLife)
            {
                // all pages shall be alive : insufficient cache
                var cacheTexture = new CacheTexture();
                cacheTexture.m_physicalRTTexture = m_cacheTextures.Count % 4 == 0
                                                   ? new RenderTarget2D(GameApp.Instance.GraphicsDevice, CacheTextureSize, CacheTextureSize, true, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents)
                                                   : m_cacheTextures.Last().m_physicalRTTexture;
                cacheTexture.m_pages[0].m_timeStamp = m_timeStamp;
                m_cacheTextures.Add(cacheTexture);
                return (m_cacheTextures.Count - 1) * PagesInOneCacheTexture;
            }
            else
            {
                TouchPage(oldestPageIndex);
                return oldestPageIndex;
            }
        }

        private void TouchPage(int page)
        {
            var textureId = page / PagesInOneCacheTexture;
            var index = page % PagesInOneCacheTexture;
            m_cacheTextures[textureId].m_pages[index].m_timeStamp = m_timeStamp;
        }

        private SystemDrawing.RectangleF MeasureCharacter(char character, SystemDrawing.Font font)
        {
            var chRegion = m_measureContext.MeasureCharacterRanges(character.ToString(), font, new SystemDrawing.RectangleF(0, 0, 0, 0), m_measureFormat);
            return chRegion[0].GetBounds(m_measureContext);
        }

        private void Initialize_Atlas()
        {
            using (var empty = new SystemDrawing.Bitmap(1, 1))
            {
                m_measureContext = SystemDrawing.Graphics.FromImage(empty);
                m_measureContext.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }

            m_measureFormat = new SystemDrawing.StringFormat(SystemDrawing.StringFormatFlags.NoClip | SystemDrawing.StringFormatFlags.NoWrap);
            m_measureFormat.SetMeasurableCharacterRanges(new SystemDrawing.CharacterRange[] { new SystemDrawing.CharacterRange(0, 1) });

            m_techBlit = m_effect.Techniques["BlitToRT"];
            m_vertDeclBlit = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

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
            m_channelMasks.ForEach(channel => channel.Dispose());
            m_vertDeclBlit.Dispose();
            m_measureContext.Dispose();
        }
    }
}
