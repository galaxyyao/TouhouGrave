using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemFont = System.Drawing.Font;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        private struct FontMetrics
        {
            public string m_id;
            public float m_spaceWidth;
        }

        private List<FontMetrics> m_registeredFonts = new List<FontMetrics>();

        private int GetFontId(SystemFont font)
        {
            var fontName = font.OriginalFontName ?? font.Name;
            var fontSize = font.Size;
            var fontStyle = font.Style;
            var fontId = String.Intern(fontName + fontSize.ToString() + fontStyle.ToString());

            var index = m_registeredFonts.FindIndex(fm => Object.ReferenceEquals(fm.m_id, fontId));
            if (index == -1)
            {
                m_registeredFonts.Add(new FontMetrics
                {
                    m_id = fontId,
                    m_spaceWidth = MeasureCharacter(' ', font).Width
                });
                index = m_registeredFonts.Count - 1;
            }

            if (index > 0xffff)
            {
                throw new OverflowException("Number of registered font has grown ridiculously too large.");
            }

            return index;
        }
    }
}
