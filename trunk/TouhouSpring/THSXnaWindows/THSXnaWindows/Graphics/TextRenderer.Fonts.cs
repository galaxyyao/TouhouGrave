﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        private struct FontMetrics
        {
            public string m_id;
            public float m_spaceWidth;
            public Font m_fontObject;
        }

        public struct FontDescriptor
        {
            public string FamilyName;
            public float Size;
            public FontStyle Style;

            public FontDescriptor(string familyName, float size)
                : this(familyName, size, FontStyle.Regular)
            { }

            public FontDescriptor(string familyName, float size, FontStyle style)
            {
                FamilyName = familyName;
                Size = size;
                Style = style;
            }

            public string Id
            {
                get { return FamilyName + Size.ToString() + Style.ToString(); }
            }
        }

        private List<FontMetrics> m_registeredFonts = new List<FontMetrics>();

        private int GetFontId(FontDescriptor fd)
        {
            var fontId = String.Intern(fd.Id);

            var index = m_registeredFonts.FindIndex(fm => Object.ReferenceEquals(fm.m_id, fontId));
            if (index == -1)
            {
                var fontObject = new Font(fd.FamilyName, fd.Size, fd.Style);
                m_registeredFonts.Add(new FontMetrics
                {
                    m_id = fontId,
                    m_fontObject = fontObject,
                    m_spaceWidth = MeasureSpace(fontObject).Width
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
