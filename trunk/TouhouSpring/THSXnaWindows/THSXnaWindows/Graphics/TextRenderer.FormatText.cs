﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SystemFont = System.Drawing.Font;

namespace TouhouSpring.Graphics
{
    partial class TextRenderer
    {
        public enum Alignment
        {
            LeftTop,
            CenterTop,
            RightTop,
            LeftMiddle,
            CenterMiddle,
            RightMiddle,
            LeftBottom,
            CenterBottom,
            RightBottom
        }

        public struct FormatOptions
        {
            public FontDescriptor Font;
            public FontDescriptor AnsiFont;
            public Alignment Alignment;
            public float CharSpacing;
            public float LineSpacing;
            public int TabSpaces;

            public FormatOptions(FontDescriptor font) : this(font, font)
            { }

            public FormatOptions(FontDescriptor font, FontDescriptor ansiFont)
            {
                Font = font;
                AnsiFont = ansiFont;
                Alignment = TextRenderer.Alignment.LeftTop;
                CharSpacing = 0;
                LineSpacing = 0;
                TabSpaces = 4;
            }
        }

        public interface IFormattedText
        {
            string Text { get; }
            FormatOptions FormatOptions { get; }
            Point Offset { get; set; }
            Size Size { get; }
        }

        private class FormattedGlyph
        {
            public Vector2 m_pos;
            public Color m_color;
            public char m_glyph;
        }

        private class FormattedText : IFormattedText
        {
            public class FormattedLine
            {
                public Vector2 m_offset;
                public FormattedGlyph[] m_glyphs;
            }

            public string Text { get; set; }
            public FormatOptions FormatOptions { get; set; }
            public Point Offset { get; set; }
            public Size Size { get; set; }
            public FormattedLine[] m_lines;

            public IEnumerable<FormattedGlyph> Glyphs()
            {
                foreach (var line in m_lines)
                {
                    foreach (var glyph in line.m_glyphs)
                    {
                        yield return glyph;
                    }
                }
            }
        }

        public IFormattedText FormatText(string text, FormatOptions formatOptions)
        {
            var colorStack = new Stack<Color>();
            colorStack.Push(Color.White);
            var fontMetrics = m_registeredFonts[GetFontId(formatOptions.Font)];
            var ansiFontMetrics = m_registeredFonts[GetFontId(formatOptions.AnsiFont)];

            float currentX = 0;
            float currentY = 0;

            // ensures the last line
            string textCopy = text.EndsWith("\n") ? text : text + "\n";

            var charArray = textCopy.ToArray();
            var glyphs = new List<FormattedGlyph>();
            var lines = new List<FormattedText.FormattedLine>();
            var maxLineWidth = 0.0f;
            var lineHeight = Math.Max(fontMetrics.m_fontObject.Height, ansiFontMetrics.m_fontObject.Height);
            var lineSpacing = formatOptions.LineSpacing + lineHeight;

            for (int i = 0; i < charArray.Length; ++i)
            {
                var ch = charArray[i];
                if (ch == ' ')
                {
                    currentX += ansiFontMetrics.m_spaceWidth;
                }
                else if (ch == '\x3000')
                {
                    currentX += fontMetrics.m_fullWidthSpaceWidth;
                }
                else if (ch == '\t')
                {
                    currentX += ansiFontMetrics.m_spaceWidth * formatOptions.TabSpaces;
                }
                else if (ch == '\n')
                {
                    var line = new FormattedText.FormattedLine();
                    line.m_glyphs = glyphs.ToArray();
                    line.m_offset = new Vector2(0, currentY);

                    var lineWidth = Math.Max(currentX - formatOptions.CharSpacing, 0);
                    maxLineWidth = Math.Max(maxLineWidth, lineWidth);

                    if (formatOptions.Alignment == Alignment.CenterTop
                        || formatOptions.Alignment == Alignment.CenterMiddle
                        || formatOptions.Alignment == Alignment.CenterBottom)
                    {
                        line.m_offset.X = -lineWidth * 0.5f;
                    }
                    else if (formatOptions.Alignment == Alignment.RightTop
                             || formatOptions.Alignment == Alignment.RightMiddle
                             || formatOptions.Alignment == Alignment.RightBottom)
                    {
                        line.m_offset.X = -lineWidth;
                    }

                    currentY += lineSpacing;
                    currentX = 0;
                    glyphs.Clear();
                    lines.Add(line);
                }
                else
                {
                    if (ch == '[' && i + 1 < charArray.Length)
                    {
                        if (charArray[i + 1] != '[')
                        {
                            int j = i + 1;
                            for (; j < charArray.Length; ++j)
                            {
                                if (charArray[j] == ']')
                                {
                                    break;
                                }
                            }
                            string token = new string(charArray, i + 1, j - i - 1);
                            if (token.StartsWith("color:", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string colorCode = token.Substring(6);
                                // use Style library color syntax
                                var color = Style.Values.Color.Parse(colorCode);
                                colorStack.Push(new Color(color.Red, color.Green, color.Blue, color.Alpha));
                            }
                            else if (token == "/color" && colorStack.Count > 1)
                            {
                                colorStack.Pop();
                            }

                            i = j;
                            continue;
                        }
                        else
                        {
                            ++i;
                        }
                    }

                    var glyphData = Load(ch, formatOptions);
                    var fg = new FormattedGlyph
                    {
                        m_pos = new Vector2(currentX, 0),
                        m_color = colorStack.Peek(),
                        m_glyph = ch,
                    };
                    glyphs.Add(fg);
                    currentX += glyphData.m_glyphSize.Width + formatOptions.CharSpacing;
                }
            }

            var offsetY = 0.0f;
            var textHeight = lines.Count > 0
                             ? lineSpacing * (lines.Count - 1) + lineHeight
                             : 0;
            if (formatOptions.Alignment == Alignment.LeftMiddle
                || formatOptions.Alignment == Alignment.CenterMiddle
                || formatOptions.Alignment == Alignment.RightMiddle)
            {
                offsetY = -textHeight * 0.5f;
            }
            else if (formatOptions.Alignment == Alignment.LeftBottom
                     || formatOptions.Alignment == Alignment.CenterBottom
                     || formatOptions.Alignment == Alignment.RightBottom)
            {
                offsetY = -textHeight;
            }

            return new FormattedText
            {
                Text = text,
                FormatOptions = formatOptions,
                Offset = new Point(0, offsetY),
                Size = new Size(maxLineWidth, textHeight),
                m_lines = lines.ToArray()
            };
        }
    }
}
