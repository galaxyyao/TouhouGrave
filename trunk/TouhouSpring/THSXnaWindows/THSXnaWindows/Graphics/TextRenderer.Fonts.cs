using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GdiFont = System.Drawing.Font;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        private List<string> m_registeredFonts = new List<string>();

        private uint GetGlyphId(char glyph, GdiFont font)
        {
            var fontName = font.OriginalFontName ?? font.Name;
            var fontSize = font.Size;
            var fontStyle = font.Style;
            var fontStr = String.Intern(fontName + fontSize.ToString() + fontStyle.ToString());

            var fontId = m_registeredFonts.FindIndex(rf => Object.ReferenceEquals(rf, fontStr));
            if (fontId == -1)
            {
                m_registeredFonts.Add(fontStr);
                fontId = m_registeredFonts.Count - 1;
            }

            if (fontId > 0xffff)
            {
                throw new OverflowException("Number of registered font has grown ridiculously too large.");
            }

            return ((uint)fontId << 16) + glyph;
        }
    }
}
